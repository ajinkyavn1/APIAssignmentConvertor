using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Interfaces;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/[controller]s")]
[Produces("application/json")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IValidator<ConversionRequest> _validator;

    public ConversionController(
        IConversionService conversionService,
        IValidator<ConversionRequest> validator)
    {
        _conversionService = conversionService;
        _validator = validator;
    }

    /// <summary>
    /// Convert a value from one unit to another
    /// </summary>
    /// <param name="request">Conversion request with value, fromUnit, and toUnit</param>
    /// <returns>Conversion response with the converted result</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConversionResponse>> Convert(ConversionRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                error = "Validation failed",
                details = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        try
        {
            var result = _conversionService.Convert(request.Value, request.FromUnit, request.ToUnit);
            var response = new ConversionResponse
            {
                Value = request.Value,
                FromUnit = request.FromUnit,
                ToUnit = request.ToUnit,
                Result = result
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
