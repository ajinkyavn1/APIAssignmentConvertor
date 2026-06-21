# Unit Conversion API

A production-oriented ASP.NET Core 8 Web API for converting values between different units of measurement.

The solution is designed with maintainability, extensibility, testability, and scalability in mind while keeping the implementation simple enough for the current requirements.

---

## Features

### Supported Categories

#### Length

* Meter
* Kilometer
* Centimeter
* Millimeter
* Inch
* Foot
* Yard
* Mile

#### Weight / Mass

* Kilogram
* Gram
* Pound
* Ounce

#### Temperature

* Celsius
* Fahrenheit
* Kelvin

#### Volume

* Liter
* Milliliter
* Gallon

---

## Technology Stack

* ASP.NET Core 8
* .NET 8
* Swagger / OpenAPI
* FluentValidation
* OpenTelemetry
* xUnit
* FluentAssertions
* Moq
* Docker
* GitHub Actions

---

# System Architecture

```text
+-----------------------------------------------------+
|                     Client                          |
+-----------------------------------------------------+
                          |
                          v
+-----------------------------------------------------+
|                 UnitConversion.Api                  |
|                                                     |
|  Controllers                                        |
|  Middleware                                         |
|  Swagger                                            |
|  OpenTelemetry                                      |
+-----------------------------------------------------+
                          |
                          v
+-----------------------------------------------------+
|             UnitConversion.Application              |
|                                                     |
|  DTOs                                               |
|  Validators                                         |
|  Interfaces                                         |
|  Services                                           |
+-----------------------------------------------------+
             |                         |
             |                         |
             v                         v
+----------------------+   +--------------------------+
| UnitConversion.Domain|   |UnitConversion.Infrastructure|
|                      |   |                            |
| Entities             |   | Repositories              |
| Enums                |   | JSON Storage              |
+----------------------+   +----------------------------+
                                        |
                                        v
                               +------------------+
                               |   units.json     |
                               +------------------+
```

---

# Project Structure

```text
UnitConversionApi
│
├── src
│   ├── UnitConversion.Api
│   ├── UnitConversion.Application
│   ├── UnitConversion.Domain
│   └── UnitConversion.Infrastructure
│
├── tests
│   └── UnitConversion.Tests
│
├── Dockerfile
├── docker-compose.yml
├── README.md
│
└── .github
    └── workflows
        └── build.yml
```

---

# Request Flow

```text
Client
   |
   | POST /api/conversions
   v
ConversionController
   |
   v
ConversionService
   |
   +----------------------+
   |                      |
   v                      v
Source Unit        Target Unit
Repository          Repository
   |                   |
   +--------+  +-------+
            |  |
            v  v
          units.json
               |
               v
      Conversion Engine
               |
               v
          API Response
```

---

# Conversion Strategy

The application uses a Base Unit Conversion Strategy.

This avoids creating conversion rules for every possible pair of units.

## Example

Instead of:

```text
meter -> foot
meter -> yard
meter -> inch
foot -> yard
foot -> inch
yard -> inch
```

The system performs:

```text
Source Unit
     |
     v
 Base Unit
     |
     v
Target Unit
```

Example:

```text
Foot
  |
  v
Meter
  |
  v
Kilometer
```

Benefits:

* Simpler implementation
* Fewer conversion rules
* Easier maintenance
* Better scalability

---

# Temperature Conversion

Temperature conversions require dedicated formulas.

Example:

```text
Celsius
   |
   v
Conversion Formula
   |
   v
Fahrenheit
```

Unlike length and weight conversions, temperature cannot be represented using a multiplication factor alone.

---

# Dynamic Unit Storage

Unit definitions are loaded from:

```text
src/UnitConversion.Infrastructure/Data/units.json
```

Example:

```json
{
  "name": "yard",
  "category": "Length",
  "factorToBaseUnit": 0.9144
}
```

Benefits:

* No code changes required
* Configuration-driven design
* Easy maintenance
* Future database migration path

---

# API Endpoints

## Convert Units

### Request

```http
POST /api/conversions
```

Request Body

```json
{
  "value": 100,
  "fromUnit": "meter",
  "toUnit": "foot"
}
```

Response

```json
{
  "value": 100,
  "fromUnit": "meter",
  "toUnit": "foot",
  "result": 328.084
}
```

---

## Get All Units

```http
GET /api/units
```

---

## Add New Unit

```http
POST /api/units
```

Request

```json
{
  "name": "yard",
  "category": "Length",
  "factorToBaseUnit": 0.9144
}
```

---

# Validation

Validation is implemented using FluentValidation.

Examples:

* Missing unit names
* Invalid categories
* Empty values
* Unsupported requests

Example:

```json
{
  "errors": {
    "fromUnit": [
      "FromUnit is required."
    ]
  }
}
```

---

# Error Handling

Global exception middleware provides consistent error responses.

Example:

```json
{
  "error": "Units belong to different categories."
}
```

---

# OpenTelemetry Observability

The API is instrumented using OpenTelemetry.

```text
                 +------------------+
                 | ASP.NET Core API |
                 +------------------+
                           |
          +----------------+----------------+
          |                                 |
          v                                 v
     Metrics                           Traces
          |                                 |
          v                                 v
    Prometheus                        Jaeger
          |
          v
      Grafana
```

Currently configured:

* ASP.NET Core instrumentation
* HTTP Client instrumentation
* Metrics collection
* Distributed tracing

---

# CI/CD Pipeline

```text
Developer
    |
    v
GitHub Push
    |
    v
GitHub Actions
    |
    +---- Restore
    |
    +---- Build
    |
    +---- Test
    |
    +---- Docker Build
    |
    v
Pipeline Success
```

Pipeline runs on:

* Push
* Pull Request

---

# Testing Strategy

Testing is implemented using:

* xUnit
* FluentAssertions
* Moq

Coverage includes:

### Conversion Service

* Meter → Foot
* Foot → Meter
* Kilogram → Pound
* Pound → Kilogram
* Celsius → Fahrenheit
* Fahrenheit → Celsius
* Kelvin → Celsius

### Validators

* Valid Requests
* Invalid Requests

### Controllers

* Successful Conversion
* Validation Failure
* Error Responses

Run tests:

```bash
dotnet test
```

---

# Running Locally

## Prerequisites

* .NET 8 SDK
* Git

Verify installation:

```bash
dotnet --version
```

---

## Clone Repository

```bash
git clone https://github.com/<your-username>/UnitConversionApi.git

cd UnitConversionApi
```

---

## Restore Packages

```bash
dotnet restore
```

---

## Build

```bash
dotnet build
```

---

## Run

```bash
dotnet run --project src/UnitConversion.Api
```

---

## Swagger

```text
https://localhost:5001/swagger
```

---

# Docker

## Build

```bash
docker build -t unit-conversion-api .
```

## Run

```bash
docker run -p 8080:8080 unit-conversion-api
```

Swagger:

```text
http://localhost:8080/swagger
```

---

# Docker Compose

Start:

```bash
docker compose up --build
```

Stop:

```bash
docker compose down
```

---
## Docker — production vs development

This section gives exact commands for running the service in two modes:

- Production: optimized Release image, no Swagger, listens on port 8080 inside the container.
- Development: Debug image (built from `Dockerfile.dev`) which includes Swagger/Swashbuckle and is convenient for interactive testing.

Both approaches are provided as docker-compose examples and as plain docker commands (useful in CI or simple hosts).

### Production (recommended for deployments)

Build the production image (multi-stage Release):

```bash
# from repo root
docker build -f Dockerfile -t unitconversionapi:latest .
```

Run the production image (bind host port 8080 to container 8080, persist units data):

```bash
docker run -d \
  --name unitconversionapi \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://+:8080 \
  -p 8080:8080 \
  -v "$PWD/src/UnitConversion.Infrastructure/Data:/app/Data" \
  unitconversionapi:latest
```

Or use docker-compose (already included):

Run the Development image (bind host port 8080 to container 8080, persist units data):

```bash
docker build -f Dockerfile.dev -t unitconversionapi:latest .
```
```bash
docker run -d \
  --name unitconversionapi \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:8080 \
  -p 8080:8080 \
  -v "$PWD/src/UnitConversion.Infrastructure/Data:/app/Data" \
  unitconversionapi:latest
```
# Scalability Considerations
The current implementation uses a Base Unit Conversion Strategy because it provides the best balance between:

* Simplicity
* Performance
* Maintainability

Future evolution could include a graph-based conversion engine.

Example:

```text
 Meter
   |
   +-------- Foot
                |
                +-------- Inch
                           |
                           +------ Centimeter
                                       |
                                       +------ Millimeter
```

This would allow conversion paths to be discovered dynamically.

The current requirements do not justify that complexity, so the simpler and more maintainable approach was chosen.

---

# Future Improvements

* Database-backed unit storage
* API Versioning
* Authentication & Authorization
* Rate Limiting
* Distributed Cache
* Graph-Based Conversion Engine
* Custom Formula Support
* Health Checks
* Kubernetes Deployment
* Advanced OpenTelemetry Dashboards
* Serilog Structured Logging

---

# Trade-offs

## Why JSON Instead of a Database?

Advantages:

* Simple
* Lightweight
* Easy to review
* Easy to modify
* No external dependencies

For the current scope, JSON provides the best balance between simplicity and flexibility.

---

## Why Base Units Instead of Graph Traversal?

Graph traversal is more flexible but introduces significant complexity.

Base units provide:

* Faster implementation
* Easier maintenance
* Simpler testing
* Clearer business logic

---

# Production Readiness

Implemented:

✓ Dependency Injection

✓ Validation

✓ Exception Handling

✓ Automated Testing

✓ OpenTelemetry

✓ Docker

✓ CI/CD Pipeline

✓ Swagger Documentation

Potential Production Enhancements:

* Serilog
* API Versioning
* Authentication
* Health Checks
* Kubernetes
* Distributed Caching

---


The architecture demonstrates:

* Clean Code Principles
* Separation of Concerns
* Testability
* Observability
* Containerization
* Scalability Considerations

while remaining straightforward to understand, maintain, and extend.
