using FluentValidation;
using UnitConversion.Application.DTOs;
using UnitConversion.Application.Interfaces;
using UnitConversion.Application.Services;
using UnitConversion.Application.Validators;
using UnitConversion.Infrastructure.Repositories;

namespace UnitConversion.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitConversionServices(this IServiceCollection services)
    {
        // Infrastructure
        services.AddSingleton<IUnitRepository, JsonUnitRepository>();

        // Application
        services.AddScoped<IConversionService, ConversionService>();

        // Validation
        services.AddScoped<IValidator<ConversionRequest>, ConversionRequestValidator>();
        services.AddScoped<IValidator<CreateUnitRequest>, CreateUnitRequestValidator>();

        return services;
    }
}
