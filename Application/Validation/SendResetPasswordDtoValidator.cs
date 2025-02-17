using Application.Dtos.AuthDtos;
using FluentValidation;

namespace Application.Validation;

public class SendResetPasswordDtoValidator : AbstractValidator<SendResetPasswordDto>
{
    public SendResetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат Email");
    }
}