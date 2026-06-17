using FluentAssertions;
using Moq;
using UnitConversion.Application.Interfaces;
using UnitConversion.Application.Services;
using UnitConversion.Domain.Entities;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Tests.Services;

public class ConversionServiceTests
{
    private readonly Mock<IUnitRepository> _mockRepository;
    private readonly ConversionService _conversionService;

    public ConversionServiceTests()
    {
        _mockRepository = new Mock<IUnitRepository>();
        _conversionService = new ConversionService(_mockRepository.Object);
    }

    [Fact]
    public void Convert_MeterToFoot_ReturnsCorrectConversion()
    {
        // Arrange
        var meter = new UnitDefinition { Name = "meter", Category = ConversionCategory.Length, FactorToBaseUnit = 1.0 };
        var foot = new UnitDefinition { Name = "foot", Category = ConversionCategory.Length, FactorToBaseUnit = 0.3048 };

        _mockRepository.Setup(r => r.GetUnitByName("meter")).Returns(meter);
        _mockRepository.Setup(r => r.GetUnitByName("foot")).Returns(foot);

        // Act
        var result = _conversionService.Convert(100, "meter", "foot");

        // Assert
        result.Should().BeApproximately(328.084, 0.001);
    }

    [Fact]
    public void Convert_FootToMeter_ReturnsCorrectConversion()
    {
        // Arrange
        var meter = new UnitDefinition { Name = "meter", Category = ConversionCategory.Length, FactorToBaseUnit = 1.0 };
        var foot = new UnitDefinition { Name = "foot", Category = ConversionCategory.Length, FactorToBaseUnit = 0.3048 };

        _mockRepository.Setup(r => r.GetUnitByName("meter")).Returns(meter);
        _mockRepository.Setup(r => r.GetUnitByName("foot")).Returns(foot);

        // Act
        var result = _conversionService.Convert(328.084, "foot", "meter");

        // Assert
        result.Should().BeApproximately(100, 0.001);
    }

    [Fact]
    public void Convert_KilogramToPound_ReturnsCorrectConversion()
    {
        // Arrange
        var kilogram = new UnitDefinition { Name = "kilogram", Category = ConversionCategory.Weight, FactorToBaseUnit = 1.0 };
        var pound = new UnitDefinition { Name = "pound", Category = ConversionCategory.Weight, FactorToBaseUnit = 0.453592 };

        _mockRepository.Setup(r => r.GetUnitByName("kilogram")).Returns(kilogram);
        _mockRepository.Setup(r => r.GetUnitByName("pound")).Returns(pound);

        // Act
        var result = _conversionService.Convert(100, "kilogram", "pound");

        // Assert
        result.Should().BeApproximately(220.462, 0.001);
    }

    [Fact]
    public void Convert_PoundToKilogram_ReturnsCorrectConversion()
    {
        // Arrange
        var kilogram = new UnitDefinition { Name = "kilogram", Category = ConversionCategory.Weight, FactorToBaseUnit = 1.0 };
        var pound = new UnitDefinition { Name = "pound", Category = ConversionCategory.Weight, FactorToBaseUnit = 0.453592 };

        _mockRepository.Setup(r => r.GetUnitByName("kilogram")).Returns(kilogram);
        _mockRepository.Setup(r => r.GetUnitByName("pound")).Returns(pound);

        // Act
        var result = _conversionService.Convert(220.462, "pound", "kilogram");

        // Assert
        result.Should().BeApproximately(100, 0.001);
    }

    [Fact]
    public void Convert_CelsiusToFahrenheit_ReturnsCorrectConversion()
    {
        // Arrange
        var celsius = new UnitDefinition { Name = "celsius", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 };
        var fahrenheit = new UnitDefinition { Name = "fahrenheit", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 };

        _mockRepository.Setup(r => r.GetUnitByName("celsius")).Returns(celsius);
        _mockRepository.Setup(r => r.GetUnitByName("fahrenheit")).Returns(fahrenheit);

        // Act
        var result = _conversionService.Convert(0, "celsius", "fahrenheit");

        // Assert
        result.Should().Be(32);
    }

    [Fact]
    public void Convert_FahrenheitToCelsius_ReturnsCorrectConversion()
    {
        // Arrange
        var celsius = new UnitDefinition { Name = "celsius", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 };
        var fahrenheit = new UnitDefinition { Name = "fahrenheit", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 };

        _mockRepository.Setup(r => r.GetUnitByName("celsius")).Returns(celsius);
        _mockRepository.Setup(r => r.GetUnitByName("fahrenheit")).Returns(fahrenheit);

        // Act
        var result = _conversionService.Convert(32, "fahrenheit", "celsius");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Convert_KelvinToCelsius_ReturnsCorrectConversion()
    {
        // Arrange
        var celsius = new UnitDefinition { Name = "celsius", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 };
        var kelvin = new UnitDefinition { Name = "kelvin", Category = ConversionCategory.Temperature, FactorToBaseUnit = 1.0 };

        _mockRepository.Setup(r => r.GetUnitByName("celsius")).Returns(celsius);
        _mockRepository.Setup(r => r.GetUnitByName("kelvin")).Returns(kelvin);

        // Act
        var result = _conversionService.Convert(273.15, "kelvin", "celsius");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Convert_DifferentCategories_ThrowsInvalidOperationException()
    {
        // Arrange
        var meter = new UnitDefinition { Name = "meter", Category = ConversionCategory.Length, FactorToBaseUnit = 1.0 };
        var kilogram = new UnitDefinition { Name = "kilogram", Category = ConversionCategory.Weight, FactorToBaseUnit = 1.0 };

        _mockRepository.Setup(r => r.GetUnitByName("meter")).Returns(meter);
        _mockRepository.Setup(r => r.GetUnitByName("kilogram")).Returns(kilogram);

        // Act & Assert
        _conversionService.Invoking(s => s.Convert(100, "meter", "kilogram"))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*different categories*");
    }

    [Fact]
    public void Convert_UnknownFromUnit_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetUnitByName("unknown")).Returns((UnitDefinition?)null);

        // Act & Assert
        _conversionService.Invoking(s => s.Convert(100, "unknown", "meter"))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public void Convert_UnknownToUnit_ThrowsInvalidOperationException()
    {
        // Arrange
        var meter = new UnitDefinition { Name = "meter", Category = ConversionCategory.Length, FactorToBaseUnit = 1.0 };
        _mockRepository.Setup(r => r.GetUnitByName("meter")).Returns(meter);
        _mockRepository.Setup(r => r.GetUnitByName("unknown")).Returns((UnitDefinition?)null);

        // Act & Assert
        _conversionService.Invoking(s => s.Convert(100, "meter", "unknown"))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*not found*");
    }
}
