# Unit Conversion API

A production-quality ASP.NET Core 8 Web API for converting values between different measurement units.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
# Unit Conversion API

A small, reliable ASP.NET Core Web API for converting between measurement units.

Targets: .NET 10.

What this repo contains
- Layered solution (API → Application → Infrastructure → Domain)
- A JSON-based unit store (Data/units.json) with runtime unit registration
- Validation (FluentValidation), global error handling, and unit tests
- Docker support and a simple docker-compose to run the service

Quick run (local)

1. Build and test:

```bash
dotnet build UnitConversion.sln -c Release
dotnet test UnitConversion.sln -c Release
```

2. Run the API:

```bash
dotnet run --project src/UnitConversion.Api/UnitConversion.Api.csproj
```

3. Try endpoints:

```bash
curl http://localhost:8080/api/units
curl -X POST http://localhost:8080/api/conversions \
  -H "Content-Type: application/json" \
  -d '{"value":100,"fromUnit":"meter","toUnit":"foot"}'
```

Run with Docker

```bash
docker compose build
docker compose up -d
curl http://localhost:8080/api/units
```

Notes
- The image uses .NET 10 runtime. Swagger is available for development only.
- Production image runs on HTTP port 8080 and reads units from `/app/Data`.

If you prefer a short development-only docker-compose (mounting code, enabling Swagger), I can add it.

License: MIT

  - Service exceptions

- **Validators** (10+ test cases)
  - Valid requests
  - Negative values
  - Empty/null inputs
  - Invalid categories

## OpenTelemetry

### Configuration

OpenTelemetry is configured in `OpenTelemetryExtensions.cs` with:

- **Tracing**: ASP.NET Core and HTTP client instrumentation
- **Metrics**: Request duration, rate, and size metrics
- **Exporter**: Console exporter for development (can be replaced with OTLP, Jaeger, etc.)

### Viewing Traces

In development mode, traces are exported to the console. For production, configure an external collector:

```csharp
tracerProvider.AddOtlpExporter(opt =>
{
    opt.Endpoint = new Uri("http://otel-collector:4318");
});
```

### Example Trace

```
Activity.TraceId:            4bf92f3577b34da6a3ce929d0e0e4736
Activity.SpanId:             00f067aa0ba902b7
Activity.Kind:               Server
Activity.DisplayName:        POST /api/conversions
Activity.Duration:           00:00:00.0523456
Activity.Status:             Ok
```

## Design Decisions

### 1. Base Unit Strategy

**Why**: Simplifies conversion logic and prevents precision loss from intermediate conversions.

**How**: Each unit stores a factor to convert to its category's base unit (e.g., meter for length). Conversions multiply by the source factor and divide by the target factor.

**Example**:
```
100 feet to meters:
  = (100 * 0.3048) / 1.0
  = 30.48 meters
```

### 2. Dedicated Temperature Logic

**Why**: Temperature conversions require formulas, not just multiplication/division.

**How**: `ConversionService` detects temperature units and applies formulas:
- Celsius → Fahrenheit: `(C * 9/5) + 32`
- Fahrenheit → Celsius: `(F - 32) * 5/9`
- Kelvin → Celsius: `K - 273.15`

### 3. JSON-based Unit Storage

**Why**: Simple, human-readable, no database dependency, versioning via git.

**How**: `JsonUnitRepository` loads units from `Data/units.json`, caches in memory, and persists changes back to disk.

**Tradeoff**: Single file, not distributed. For large deployments, replace with a database.

### 4. FluentValidation

**Why**: Declarative, reusable, testable validation rules.

**How**: Each DTO has a corresponding validator class inheriting `AbstractValidator<T>`.

**Example**:
```csharp
RuleFor(x => x.Value)
    .GreaterThanOrEqualTo(0)
    .WithMessage("Value must be non-negative");
```

### 5. Middleware-based Exception Handling

**Why**: Centralized error handling, consistent JSON responses.

**How**: `ExceptionMiddleware` catches all exceptions and returns a JSON error object.

**Example**:
```json
{
  "error": "Unit 'unknown' not found"
}
```

### 6. OpenTelemetry Integration

**Why**: Production-grade observability for monitoring, tracing, and debugging.

**How**: Console exporter in development; easily switchable to OTLP for production.

## Future Improvements

### Short-term

- [ ] Add JWT authentication and authorization
- [ ] Implement rate limiting (Polly or AspNetCoreRateLimit)
- [ ] Add request/response logging middleware
- [ ] Implement unit caching with expiration
- [ ] Add batch conversion endpoint
- [ ] Implement database persistence (EF Core + PostgreSQL/SQL Server)
- [ ] Add API versioning (URL-based or header-based)

### Medium-term

- [ ] Add historical conversion rates for currencies
- [ ] Implement unit presets/favorites for users
- [ ] Add conversion history and audit logs
- [ ] Create mobile-friendly web UI
- [ ] Add multi-language support (i18n)
- [ ] Implement user accounts and preferences
- [ ] Add conversion formula documentation
- [ ] Create OpenAPI client SDKs (C#, TypeScript, Python)

### Long-term

- [ ] Machine learning for unit recommendations
- [ ] Real-time conversion sync across distributed services
- [ ] Multi-region deployment with Azure App Service
- [ ] Advanced analytics dashboard
- [ ] Plugin architecture for custom converters
- [ ] Integration with third-party conversion APIs

## Tradeoffs

### Simplicity vs. Feature-richness

**Tradeoff**: The API intentionally keeps unit storage simple (JSON file) rather than introducing a database.

**Rationale**: Reduces dependencies, deployment complexity, and operational overhead. Suitable for small to medium deployments. For enterprise scale, swap `JsonUnitRepository` with `EFCoreUnitRepository`.

### Consistency vs. Performance

**Tradeoff**: Unit data is reloaded from disk on each repository instantiation; caching is in-memory per instance.

**Rationale**: Ensures newly added units are visible immediately. For high-traffic scenarios, implement distributed caching (Redis).

### Temperature vs. Base Unit Strategy

**Tradeoff**: Temperature conversions use formulas instead of the base unit factor approach.

**Rationale**: Temperature scales have non-linear relationships (Celsius 0°C ≠ 0 Fahrenheit). Using formulas is more accurate.

### Testing Framework Choice

**Tradeoff**: Using xUnit, FluentAssertions, and Moq instead of NUnit/MSTest.

**Rationale**: xUnit is modern, extensible, and widely adopted in the .NET community. FluentAssertions provides readable test assertions.

## License

MIT License - see LICENSE file for details.

---

**Author**: Your Name  
**Last Updated**: June 2026  
**Status**: Production Ready ✅
