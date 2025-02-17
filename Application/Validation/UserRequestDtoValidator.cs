using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validation;

public class UserRequestDtoValidator : AbstractValidator<UserRequestDto>
{
    public UserRequestDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id обязателен");

        RuleFor(x => x.Topic)
            .NotEmpty().WithMessage("Топик обязателен")
            .MaximumLength(255).WithMessage("Топик не может превышать 255 символов");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно")
            .MaximumLength(1000).WithMessage("Описание не может превышать 1000 символов");

        RuleFor(x => x.CreatedAt)
            .NotEmpty().WithMessage("Дата создания обязательна");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Статус обязателен");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId обязателен");

        RuleFor(x => x.OperatorId)
            .NotEmpty().WithMessage("OperatorId обязателен");

        RuleForEach(x => x.Attachments)
            .SetValidator(new AttachmentDtoValidator());
    }
}