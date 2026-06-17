namespace UnitConversion.Application.Interfaces;

public interface IConversionService
{
    double Convert(
        double value,
        string fromUnit,
        string toUnit);
}