using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validation;

public class AttachmentDtoValidator : AbstractValidator<AttachmentDto>
{
    public AttachmentDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id обязателен");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Имя файла обязательно")
            .MaximumLength(255).WithMessage("Имя файла не может превышать 255 символов");

        RuleFor(x => x.FilePath)
            .NotEmpty().WithMessage("Путь к файлу обязателен");
    }
}