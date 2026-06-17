using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitConversion.Api.Controllers;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Interfaces;

namespace UnitConversion.Tests.Controllers;

public class ConversionControllerTests
{
    private readonly Mock<IConversionService> _mockConversionService;
    private readonly Mock<IValidator<ConversionRequest>> _mockValidator;
    private readonly ConversionController _controller;

    public ConversionControllerTests()
    {
        _mockConversionService = new Mock<IConversionService>();
        _mockValidator = new Mock<IValidator<ConversionRequest>>();
        _controller = new ConversionController(_mockConversionService.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task Convert_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new ConversionRequest { Value = 100, FromUnit = "meter", ToUnit = "foot" };
        var validationResult = new ValidationResult();

        _mockValidator.Setup(v => v.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(validationResult);
        _mockConversionService.Setup(s => s.Convert(100, "meter", "foot"))
            .Returns(328.084);

        // Act
        var result = await _controller.Convert(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeOfType<ConversionResponse>();
    }

    [Fact]
    public async Task Convert_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new ConversionRequest { Value = -100, FromUnit = "", ToUnit = "foot" };
        var error = new ValidationFailure("Value", "Value must be greater than or equal to 0");
        var validationResult = new ValidationResult(new[] { error });

        _mockValidator.Setup(v => v.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Convert(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Convert_WithServiceException_ReturnsBadRequest()
    {
        // Arrange
        var request = new ConversionRequest { Value = 100, FromUnit = "unknown", ToUnit = "foot" };
        var validationResult = new ValidationResult();

        _mockValidator.Setup(v => v.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(validationResult);
        _mockConversionService.Setup(s => s.Convert(100, "unknown", "foot"))
            .Throws(new InvalidOperationException("Unit 'unknown' not found"));

        // Act
        var result = await _controller.Convert(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
