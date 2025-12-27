FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY dotnet-high-performance-api.sln ./
COPY src/ ./src/

RUN dotnet restore dotnet-high-performance-api.sln

WORKDIR /src/src/DotNetHighPerformanceApi.Api
RUN dotnet publish DotNetHighPerformanceApi.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN useradd -m appuser
COPY --from=build /app/publish/ .
USER appuser

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "DotNetHighPerformanceApi.Api.dll"]
