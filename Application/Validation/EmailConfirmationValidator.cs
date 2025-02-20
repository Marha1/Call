using Application.Dtos.AuthDtos;
using FluentValidation;

namespace Application.Validation;

public class EmailConfirmationValidator : AbstractValidator<EmailConfirmationDto>
{
    public EmailConfirmationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.ConfirmedCode)
            .NotEmpty().WithMessage("Confirmation code is required.")
            .Matches(@"^\d{6}$").WithMessage("Confirmation code must be a 6-digit number.");
    }
}