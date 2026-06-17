FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY UnitConversion.sln ./
COPY src/UnitConversion.Api/UnitConversion.Api.csproj src/UnitConversion.Api/
COPY src/UnitConversion.Application/UnitConversion.Application.csproj src/UnitConversion.Application/
COPY src/UnitConversion.Domain/UnitConversion.Domain.csproj src/UnitConversion.Domain/
COPY src/UnitConversion.Infrastructure/UnitConversion.Infrastructure.csproj src/UnitConversion.Infrastructure/

# Restore dependencies
RUN dotnet restore UnitConversion.sln

# Copy everything and build
COPY . .
WORKDIR /src/src/UnitConversion.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create Data directory for units.json
RUN mkdir -p /app/Data

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/api/units || exit 1

ENTRYPOINT ["dotnet", "UnitConversion.Api.dll"]

