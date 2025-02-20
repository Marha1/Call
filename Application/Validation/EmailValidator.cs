using Application.Dtos.AuthDtos;
using FluentValidation;

namespace Application.Validation;

public class EmailValidator : AbstractValidator<EmailConfirmationRequestDto>
{
    public EmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}