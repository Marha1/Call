using Domain.Models;
using FluentValidation;

namespace Application.Validation;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .Length(3, 20).WithMessage("Имя пользователя должно содержать от 3 до 20 символов");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат Email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .Length(2, 50).WithMessage("Имя должно содержать от 2 до 50 символов");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Фамилия обязательна")
            .Length(2, 50).WithMessage("Фамилия должна содержать от 2 до 50 символов");

        RuleFor(x => x.LastName)
            .Length(2, 50).WithMessage("Отчество должно содержать от 2 до 50 символов")
            .When(x => !string.IsNullOrEmpty(x.LastName));
    }
}