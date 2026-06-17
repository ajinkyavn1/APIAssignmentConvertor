namespace UnitConversion.Application.DTOs;

public class UnitResponse
{
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public double FactorToBaseUnit { get; set; }
}
