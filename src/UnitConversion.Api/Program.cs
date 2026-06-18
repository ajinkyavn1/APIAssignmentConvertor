using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UnitConversion.Api.Extensions;
using UnitConversion.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Register Swagger only in Debug builds (development). Release images skip Swagger to avoid
// pulling in OpenAPI/Swashbuckle assemblies that can cause runtime/compatibility problems.
#if DEBUG
builder.Services.AddSwaggerGen();
#endif

// Add Unit Conversion Services
builder.Services.AddUnitConversionServices();

// Add OpenTelemetry
builder.Services.AddOpenTelemetryTracing();

// Build app
var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionMiddleware>();

// Only enable Swagger in Debug builds (development)
#if DEBUG
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endif

// Only use HTTPS redirection in Development (containers typically run HTTP with ASPNETCORE_URLS)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

