using FluentValidation;
using UnitConversion.Application.DTOs;

namespace UnitConversion.Application.Validators;

public class CreateUnitRequestValidator : AbstractValidator<CreateUnitRequest>
{
    public CreateUnitRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .NotNull()
            .WithMessage("Name cannot be null");

        RuleFor(x => x.ConversionCategory)
            .IsInEnum()
            .WithMessage("Category must be a valid ConversionCategory");

        RuleFor(x => x.FactorToBaseUnit)
            .GreaterThan(0)
            .WithMessage("FactorToBaseUnit must be greater than 0");
    }
}
