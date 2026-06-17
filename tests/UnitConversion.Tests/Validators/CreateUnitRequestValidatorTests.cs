using FluentAssertions;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Validators;
using UnitConversion.Domain.Enums;

namespace UnitConversion.Tests.Validators;

public class CreateUnitRequestValidatorTests
{
    private readonly CreateUnitRequestValidator _validator;

    public CreateUnitRequestValidatorTests()
    {
        _validator = new CreateUnitRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_ReturnsTrue()
    {
        // Arrange
        var request = new CreateUnitRequest
        {
            Name = "yard",
            Category = ConversionCategory.Length,
            FactorToBaseUnit = 0.9144
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_ReturnsFalse()
    {
        // Arrange
        var request = new CreateUnitRequest
        {
            Name = "",
            Category = ConversionCategory.Length,
            FactorToBaseUnit = 0.9144
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithZeroFactor_ReturnsFalse()
    {
        // Arrange
        var request = new CreateUnitRequest
        {
            Name = "yard",
            Category = ConversionCategory.Length,
            FactorToBaseUnit = 0
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FactorToBaseUnit");
    }

    [Fact]
    public async Task Validate_WithNegativeFactor_ReturnsFalse()
    {
        // Arrange
        var request = new CreateUnitRequest
        {
            Name = "yard",
            Category = ConversionCategory.Length,
            FactorToBaseUnit = -0.9144
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FactorToBaseUnit");
    }
}
