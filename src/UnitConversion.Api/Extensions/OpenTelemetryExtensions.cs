using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace UnitConversion.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(this IServiceCollection services)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService("UnitConversionApi");

        services.AddOpenTelemetry()
            .WithTracing(tracerProvider =>
            {
                tracerProvider
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                    })
                    .AddHttpClientInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                    })
                    .AddConsoleExporter(opt =>
                    {
                        opt.Targets = ConsoleExporterOutputTargets.Console;
                    });
            });

        return services;
    }
}
