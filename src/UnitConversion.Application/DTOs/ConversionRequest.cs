namespace UnitConversion.Application.DTOs;

public class ConversionRequest
{
    public double Value { get; set; }

    public string FromUnit { get; set; } = string.Empty;

    public string ToUnit { get; set; } = string.Empty;
}