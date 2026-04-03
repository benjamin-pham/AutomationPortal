using FluentValidation;

namespace AutomationPortal.Application.Features.GeminiKeys.CreateGeminiKey;

public sealed class CreateGeminiKeyCommandValidator : AbstractValidator<CreateGeminiKeyCommand>
{
    public CreateGeminiKeyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.KeyValue)
            .NotEmpty().WithMessage("Key value is required.")
            .MaximumLength(500).WithMessage("Key value must not exceed 500 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User assignment is required.");
    }
}
