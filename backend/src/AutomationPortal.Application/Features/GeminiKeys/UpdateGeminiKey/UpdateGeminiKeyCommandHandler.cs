using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutomationPortal.Application.Features.GeminiKeys.UpdateGeminiKey;

public sealed class UpdateGeminiKeyCommandHandler(
    IGeminiKeyRepository geminiKeyRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateGeminiKeyCommandHandler> logger)
    : IRequestHandler<UpdateGeminiKeyCommand, Result>
{
    public async Task<Result> Handle(UpdateGeminiKeyCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating Gemini Key ID {Id} ('{Name}') for user {UserId}", request.Id, request.Name, request.UserId);

        var geminiKey = await geminiKeyRepository.GetByIdAsync(request.Id, ct);
        if (geminiKey is null)
        {
            logger.LogWarning("Gemini Key ID {Id} not found", request.Id);
            return Result.Failure(GeminiKeyErrors.NotFound);
        }

        var keyWithName = await geminiKeyRepository.GetByNameAsync(request.Name, ct);
        if (keyWithName is not null && keyWithName.Id != request.Id)
        {
            logger.LogWarning("Gemini Key name '{Name}' already exists on another record {OtherId}", request.Name, keyWithName.Id);
            return Result.Failure(GeminiKeyErrors.NameAlreadyExists);
        }

        var existingKeyForUser = await geminiKeyRepository.GetByUserIdAsync(request.UserId, ct);
        if (existingKeyForUser is not null && existingKeyForUser.Id != request.Id)
        {
            if (!request.ReplaceExisting)
            {
                logger.LogWarning("User {UserId} already has a Gemini Key", request.UserId);
                return Result.Failure(GeminiKeyErrors.UserAlreadyHasKey);
            }

            logger.LogInformation("Replacing existing Gemini Key '{OldName}' for user {UserId}", existingKeyForUser.Name, request.UserId);
            geminiKeyRepository.Remove(existingKeyForUser);
        }

        var finalKeyValue = (!string.IsNullOrWhiteSpace(request.KeyValue) && !request.KeyValue.StartsWith("****"))
            ? request.KeyValue
            : geminiKey.KeyValue;

        geminiKey.Update(request.Name, finalKeyValue, request.UserId);

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Gemini Key ID {Id} updated successfully", geminiKey.Id);

        return Result.Success();
    }
}
