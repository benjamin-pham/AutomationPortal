using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Errors;
using AutomationPortal.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutomationPortal.Application.Features.GeminiKeys.DeleteGeminiKey;

public sealed class DeleteGeminiKeyCommandHandler(
    IGeminiKeyRepository geminiKeyRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteGeminiKeyCommandHandler> logger)
    : IRequestHandler<DeleteGeminiKeyCommand, Result>
{
    public async Task<Result> Handle(DeleteGeminiKeyCommand request, CancellationToken ct)
    {
        logger.LogInformation("Deleting Gemini Key with ID {Id}", request.Id);

        var geminiKey = await geminiKeyRepository.GetByIdAsync(request.Id, ct);
        if (geminiKey is null)
        {
            logger.LogWarning("Gemini Key with ID {Id} was not found", request.Id);
            return Result.Failure(GeminiKeyErrors.NotFound);
        }

        logger.LogInformation("Gemini Key '{Name}' found, performing hard delete", geminiKey.Name);
        
        // Use Remove() for hard delete, as specified in the tasks (do NOT use SoftDelete())
        geminiKeyRepository.Remove(geminiKey);

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Gemini Key '{Name}' (ID {Id}) deleted successfully", geminiKey.Name, request.Id);

        return Result.Success();
    }
}
