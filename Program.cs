using DotNetHighPerformanceApi.Persistence;
using DotNetHighPerformanceApi.Validation;
using FluentValidation;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});

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
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// 5. AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 6. FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// 8. Repositories and Services (feature-scoped)
builder.Services.AddScoped<DotNetHighPerformanceApi.Features.Orders.v1.Repositories.IOrderRepository, DotNetHighPerformanceApi.Features.Orders.v1.Repositories.OrderRepository>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Features.Products.v1.Repositories.IProductRepository, DotNetHighPerformanceApi.Features.Products.v1.Repositories.ProductRepository>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Features.Orders.v1.Services.IOrderService, DotNetHighPerformanceApi.Features.Orders.v1.Services.OrderService>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Features.Products.v1.Services.IProductService, DotNetHighPerformanceApi.Features.Products.v1.Services.ProductService>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Caching.ICacheService, DotNetHighPerformanceApi.Caching.DistributedCacheService>();
builder.Services.AddScoped<DotNetHighPerformanceApi.Caching.IETagService, DotNetHighPerformanceApi.Caching.ETagService>();

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

app.UseResponseCompression();

app.UseHttpsRedirection();

app.UseResponseCaching(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
