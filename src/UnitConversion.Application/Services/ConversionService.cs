using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Services;

public class ConversionService : IConversionService
{
    private readonly IUnitRepository _repository;

    public ConversionService(IUnitRepository repository)
    {
        _repository = repository;
    }

    public double Convert(double value, string fromUnit, string toUnit)
    {
        var fromUnitDef = _repository.GetUnitByName(fromUnit);
        var toUnitDef = _repository.GetUnitByName(toUnit);

        if (fromUnitDef == null)
        {
            throw new InvalidOperationException($"Unit '{fromUnit}' not found");
        }

        if (toUnitDef == null)
        {
            throw new InvalidOperationException($"Unit '{toUnit}' not found");
        }

        if (fromUnitDef.Category != toUnitDef.Category)
        {
            throw new InvalidOperationException(
                $"Cannot convert between different categories: {fromUnitDef.Category} and {toUnitDef.Category}");
        }

        // Handle temperature conversions separately
        if (fromUnitDef.Category == ConversionCategory.Temperature)
        {
            return ConvertTemperature(value, fromUnit, toUnit);
        }

        // For other categories, use base unit strategy
        double baseValue = value * fromUnitDef.FactorToBaseUnit;
        double result = baseValue / toUnitDef.FactorToBaseUnit;

        return Math.Round(result, 6);
    }

    private double ConvertTemperature(double value, string fromUnit, string toUnit)
    {
        // Convert from source unit to Celsius
        double celsius = fromUnit.ToLower() switch
        {
            "celsius" => value,
            "fahrenheit" => (value - 32) * 5 / 9,
            "kelvin" => value - 273.15,
            _ => throw new InvalidOperationException($"Unknown temperature unit: {fromUnit}")
        };

        // Convert from Celsius to target unit
        double result = toUnit.ToLower() switch
        {
            "celsius" => celsius,
            "fahrenheit" => (celsius * 9 / 5) + 32,
            "kelvin" => celsius + 273.15,
            _ => throw new InvalidOperationException($"Unknown temperature unit: {toUnit}")
        };

        return Math.Round(result, 6);
    }
}
