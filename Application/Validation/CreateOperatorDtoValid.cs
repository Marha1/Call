using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validation;

public class CreateOperatorDtoValid : AbstractValidator<OperatorDto>
{
    public CreateOperatorDtoValid()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(10).WithMessage("Max length username 10 ");


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Min Length 6");

        RuleFor(x => x.DepartmentOperator)
            .NotNull().WithMessage("Password is required.");
    }
}