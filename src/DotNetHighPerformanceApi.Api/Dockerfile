# Multi-stage Dockerfile for DotNetHighPerformanceApi API (targeting .NET 10)
# This Dockerfile expects the build context to be the repository root so
# referenced projects and the solution file are available during restore.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching
# Copy solution and all project files so restore can resolve all references
COPY dotnet-high-performance-api.sln ./
COPY src/ ./src/

RUN dotnet restore dotnet-high-performance-api.sln

# Copy the rest of the repository (if any additional files outside src are needed)
COPY . .

# Publish the API project
WORKDIR /src/src/DotNetHighPerformanceApi.Api
RUN dotnet publish DotNetHighPerformanceApi.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:PublishTrimmed=true /p:TrimUnusedDependencies=true /p:PublishSingleFile=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Use a non-root user for better security
RUN useradd -m appuser && mkdir -p /app && chown -R appuser:appuser /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish/ .
USER appuser

ENTRYPOINT ["dotnet", "DotNetHighPerformanceApi.Api.dll"]

# Na raiz do repositório
# docker build -f src/DotNetHighPerformanceApi.Api/Dockerfile -t dotnethighperfapi:latest .

# Rodar localmente
# docker run --rm -p 5000:5000 dotnethighperfapi:latest