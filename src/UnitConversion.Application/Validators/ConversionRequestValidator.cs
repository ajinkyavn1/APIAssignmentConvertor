using FluentValidation;
using UnitConversion.Application.DTOs;

namespace UnitConversion.Application.Validators;

public class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    public ConversionRequestValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Value must be greater than or equal to 0");

        RuleFor(x => x.FromUnit)
            .NotEmpty()
            .WithMessage("FromUnit is required")
            .NotNull()
            .WithMessage("FromUnit cannot be null");

        RuleFor(x => x.ToUnit)
            .NotEmpty()
            .WithMessage("ToUnit is required")
            .NotNull()
            .WithMessage("ToUnit cannot be null");
    }
}
