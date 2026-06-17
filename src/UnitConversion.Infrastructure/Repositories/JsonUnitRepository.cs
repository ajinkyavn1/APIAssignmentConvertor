using System.Text.Json;
using UnitConversion.Application.Interfaces;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Infrastructure.Repositories;

public class JsonUnitRepository : IUnitRepository
{
    private readonly string _filePath;
    private List<UnitDefinition> _units;

    public JsonUnitRepository()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "units.json");
        _units = LoadUnitsFromFile();
    }

    public IEnumerable<UnitDefinition> GetAllUnits() => _units.ToList();

    public UnitDefinition? GetUnitByName(string name) => 
        _units.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public bool UnitExists(string name) => 
        _units.Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void AddUnit(string name, ConversionCategory category, double factorToBaseUnit)
    {
        if (UnitExists(name))
        {
            throw new InvalidOperationException($"Unit '{name}' already exists");
        }

        var unit = new UnitDefinition
        {
            Name = name,
            Category = category,
            FactorToBaseUnit = factorToBaseUnit
        };

        _units.Add(unit);
        SaveUnitsToFile();
    }

    private List<UnitDefinition> LoadUnitsFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return InitializeDefaultUnits();
            }

            var json = File.ReadAllText(_filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var units = JsonSerializer.Deserialize<List<UnitDefinition>>(json, options);
            return units ?? InitializeDefaultUnits();
        }
        catch
        {
            return InitializeDefaultUnits();
        }
    }

    private void SaveUnitsToFile()
    {
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_units, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save units to file: {ex.Message}", ex);
        }
    }

    private List<UnitDefinition> InitializeDefaultUnits()
    {
        return new List<UnitDefinition>
        {
            // Length - base unit: meter
            new() { Name = "meter", Category = ConversionCategory.Length, FactorToBaseUnit = 1.0 },
            new() { Name = "kilometer", Category = ConversionCategory.Length, FactorToBaseUnit = 1000.0 },
            new() { Name = "centimeter", Category = ConversionCategory.Length, FactorToBaseUnit = 0.01 },
            new() { Name = "millimeter", Category = ConversionCategory.Length, FactorToBaseUnit = 0.001 },
            new() { Name = "inch", Category = ConversionCategory.Length, FactorToBaseUnit = 0.0254 },
            new() { Name = "foot", Category = ConversionCategory.Length, FactorToBaseUnit = 0.3048 },
            new() { Name = "yard", Category = ConversionCategory.Length, FactorToBaseUnit = 0.9144 },
            new() { Name = "mile", Category = ConversionCategory.Length, FactorToBaseUnit = 1609.34 },

            // Weight - base unit: kilogram
            new() { Name = "kilogram", Category = ConversionCategory.Weight, FactorToBaseUnit = 1.0 },
            new() { Name = "gram", Category = ConversionCategory.Weight, FactorToBaseUnit = 0.001 },
            new() { Name = "pound", Category = ConversionCategory.Weight, FactorToBaseUnit = 0.453592 },
            new() { Name = "ounce", Category = ConversionCategory.Weight, FactorToBaseUnit = 0.0283495 },

            // Volume - base unit: liter
            new() { Name = "liter", Category = ConversionCategory.Volume, FactorToBaseUnit = 1.0 },
            new() { Name = "milliliter", Category = ConversionCategory.Volume, FactorToBaseUnit = 0.001 },
            new() { Name = "gallon", Category = ConversionCategory.Volume, FactorToBaseUnit = 3.78541 },

            // Temperature - special handling
            new() { Name = "celsius", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 },
            new() { Name = "fahrenheit", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 },
            new() { Name = "kelvin", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 }
        };
    }
}
