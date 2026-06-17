using UnitConversion.Domain.Enums;

namespace UnitConversion.Domain.Entities;

public class UnitDefinition
{
    public string Name { get; set; } = string.Empty;

    public ConversionCategory Category { get; set; }

    public double FactorToBaseUnit { get; set; }
}