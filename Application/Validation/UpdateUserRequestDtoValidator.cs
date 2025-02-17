using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validation;

public class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserRequestDtoValidator()
    {
        RuleFor(x => x.Topic)
            .IsInEnum().WithMessage("Неверный топик");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно")
            .MaximumLength(1000).WithMessage("Описание не может превышать 1000 символов");
    }
}