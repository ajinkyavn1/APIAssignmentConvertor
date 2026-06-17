using FluentAssertions;
using FluentValidation;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Validators;

namespace UnitConversion.Tests.Validators;

public class ConversionRequestValidatorTests
{
    private readonly ConversionRequestValidator _validator;

    public ConversionRequestValidatorTests()
    {
        _validator = new ConversionRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_ReturnsTrue()
    {
        // Arrange
        var request = new ConversionRequest { Value = 100, FromUnit = "meter", ToUnit = "foot" };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNegativeValue_ReturnsFalse()
    {
        // Arrange
        var request = new ConversionRequest { Value = -1, FromUnit = "meter", ToUnit = "foot" };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("greater than or equal to"));
    }

    [Fact]
    public async Task Validate_WithEmptyFromUnit_ReturnsFalse()
    {
        // Arrange
        var request = new ConversionRequest { Value = 100, FromUnit = "", ToUnit = "foot" };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FromUnit");
    }

    [Fact]
    public async Task Validate_WithEmptyToUnit_ReturnsFalse()
    {
        // Arrange
        var request = new ConversionRequest { Value = 100, FromUnit = "meter", ToUnit = "" };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ToUnit");
    }

    [Fact]
    public async Task Validate_WithNullFromUnit_ReturnsFalse()
    {
        // Arrange
        var request = new ConversionRequest { Value = 100, FromUnit = null!, ToUnit = "foot" };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
