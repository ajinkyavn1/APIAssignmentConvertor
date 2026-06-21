using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Interfaces;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UnitsController : ControllerBase
{
    private readonly IUnitRepository _repository;
    private readonly IValidator<CreateUnitRequest> _validator;

    public UnitsController(
        IUnitRepository repository,
        IValidator<CreateUnitRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    /// <summary>
    /// Get all available units
    /// </summary>
    /// <returns>List of all unit definitions</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<UnitResponse>> GetAllUnits()
    {
        var units = _repository.GetAllUnits().Select(u => new UnitResponse
        {
            Name = u.Name,
            Category = u.Category.ToString(),
            FactorToBaseUnit = u.FactorToBaseUnit
        }).ToList();

        return Ok(units);
    }

    /// <summary>
    /// Add a new unit dynamically
    /// </summary>
    /// <param name="request">Create unit request with name, category, and conversion factor</param>
    /// <returns>Created unit response</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UnitResponse>> CreateUnit([FromBody] CreateUnitRequest request)
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
            _repository.AddUnit(request.Name, request.ConversionCategory, request.FactorToBaseUnit);
            var response = new UnitResponse
            {
                Name = request.Name,
                Category = request.ConversionCategory.ToString(),
                FactorToBaseUnit = request.FactorToBaseUnit
            };

            return Created($"/api/units/{request.Name}", response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
