using FluentValidation;

namespace AutomationPortal.Application.Features.GeminiKeys.DeleteGeminiKey;

public sealed class DeleteGeminiKeyCommandValidator : AbstractValidator<DeleteGeminiKeyCommand>
{
    public DeleteGeminiKeyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Gemini Key ID is required.");
    }
}
