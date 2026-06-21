using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.DTOs;

public class CreateUnitRequest
{
    public string Name { get; set; } = string.Empty;

    public ConversionCategory ConversionCategory { get; set; }

    public double FactorToBaseUnit { get; set; }
}