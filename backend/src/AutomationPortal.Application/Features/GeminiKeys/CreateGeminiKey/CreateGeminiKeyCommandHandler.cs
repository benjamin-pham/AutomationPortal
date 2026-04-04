using AutomationPortal.Application.Abstractions.Messaging;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Entities;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutomationPortal.Application.Features.GeminiKeys.CreateGeminiKey;

public sealed class CreateGeminiKeyCommandHandler(
    IGeminiKeyRepository geminiKeyRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateGeminiKeyCommandHandler> logger)
    : IRequestHandler<CreateGeminiKeyCommand, Result<CreateGeminiKeyResponse>>
{
    public async Task<Result<CreateGeminiKeyResponse>> Handle(CreateGeminiKeyCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating Gemini Key '{Name}' for user {UserId}", request.Name, request.UserId);

        if (await geminiKeyRepository.GetByNameAsync(request.Name, ct) is not null)
        {
            logger.LogWarning("Gemini Key name '{Name}' already exists", request.Name);
            return Result.Failure<CreateGeminiKeyResponse>(GeminiKeyErrors.NameAlreadyExists);
        }

        var existingKey = await geminiKeyRepository.GetByUserIdAsync(request.UserId, ct);
        if (existingKey is not null)
        {
            if (!request.ReplaceExisting)
            {
                logger.LogWarning("User {UserId} already has a Gemini Key", request.UserId);
                return Result.Failure<CreateGeminiKeyResponse>(GeminiKeyErrors.UserAlreadyHasKey);
            }

            logger.LogInformation("Replacing existing Gemini Key '{OldName}' for user {UserId}", existingKey.Name, request.UserId);
            geminiKeyRepository.Remove(existingKey);
        }

        var geminiKey = GeminiKey.Create(
            request.Name,
            request.KeyValue,
            request.UserId);

        geminiKeyRepository.Add(geminiKey);

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Gemini Key '{Name}' created successfully with ID {Id}", geminiKey.Name, geminiKey.Id);

        return new CreateGeminiKeyResponse(geminiKey.Id, geminiKey.Name);
    }
}
