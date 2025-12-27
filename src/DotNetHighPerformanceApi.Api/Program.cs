using DotNetHighPerformanceApi.Infrastructure.Persistence;
using DotNetHighPerformanceApi.Application.Validation;
using FluentValidation;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Linq;


var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});
builder.Services.AddScoped<DotNetHighPerformanceApi.Application.Common.Interfaces.IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// 2. Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
});
builder.Services.AddResponseCaching();

// 3. Response Compression (Gzip + Brotli)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// 4. MediatR + Pipeline Behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(DotNetHighPerformanceApi.Application.Validation.ValidationBehavior<,>).Assembly);
    cfg.AddOpenBehavior(typeof(DotNetHighPerformanceApi.Application.Validation.ValidationBehavior<,>));
});

// 5. AutoMapper
builder.Services.AddAutoMapper(typeof(DotNetHighPerformanceApi.Application.Validation.ValidationBehavior<,>).Assembly);

// 6. FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(DotNetHighPerformanceApi.Application.Validation.ValidationBehavior<,>).Assembly);

// 8. Repositories and Services (feature-scoped)
builder.Services.AddScoped<DotNetHighPerformanceApi.Domain.Repositories.IOrderRepository, DotNetHighPerformanceApi.Infrastructure.Repositories.OrderRepository>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Domain.Repositories.IProductRepository, DotNetHighPerformanceApi.Infrastructure.Repositories.ProductRepository>();

builder.Services.AddScoped<DotNetHighPerformanceApi.Application.Features.Orders.v1.Services.IOrderService, DotNetHighPerformanceApi.Application.Features.Orders.v1.Services.OrderService>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Application.Features.Products.v1.Services.IProductService, DotNetHighPerformanceApi.Application.Features.Products.v1.Services.ProductService>();

builder.Services.AddScoped<DotNetHighPerformanceApi.Application.Caching.ICacheService, DotNetHighPerformanceApi.Infrastructure.Caching.DistributedCacheService>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Application.Caching.IETagService, DotNetHighPerformanceApi.Infrastructure.Caching.ETagService>();

// 9. Health Checks (Postgres + Redis)
var postgresConnStr = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada");
var redisConnStr = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";

builder.Services.AddHealthChecks()
    .AddNpgSql(postgresConnStr, name: "postgres", tags: new[] { "db" })
    .AddRedis(redisConnStr, name: "redis", tags: new[] { "cache" });


// 7. Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
}).AddMvc().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Auto-migrate for demo purposes.
    // In production, use CI/CD or bundles.
    using var scope = app.Services.CreateScope();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated(); // Creates DB and Seed data if not exists
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating DB: {ex.Message}");
    }
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseResponseCaching();
app.UseAuthorization();

app.MapControllers();

// Liveness probe - simple check that the app is running
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));

// Readiness probe - checks postgres and redis
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                error = e.Value.Exception?.Message,
                duration = e.Value.Duration.ToString()
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();

public partial class Program { }
