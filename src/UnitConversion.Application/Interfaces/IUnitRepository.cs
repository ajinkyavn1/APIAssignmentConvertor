using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Application.Interfaces;

public interface IUnitRepository
{
    IEnumerable<UnitDefinition> GetAllUnits();
    UnitDefinition? GetUnitByName(string name);
    bool UnitExists(string name);
    void AddUnit(string name, ConversionCategory category, double factorToBaseUnit);
}
