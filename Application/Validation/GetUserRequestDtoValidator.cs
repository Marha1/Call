using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validation;

public class GetUserRequestDtoValidator : AbstractValidator<GetUserRequestDto>
{
    public GetUserRequestDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id обязателен");

        RuleFor(x => x.Topic)
            .IsInEnum().WithMessage("Неверный топик");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно")
            .MaximumLength(1000).WithMessage("Описание не может превышать 1000 символов");

        RuleFor(x => x.CreatedDate)
            .NotEmpty().WithMessage("Дата создания обязательна");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Неверный статус");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId обязателен");
    }
}