using DotNetHighPerformanceApi.Application.Common.Interfaces;
using DotNetHighPerformanceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetHighPerformanceApi.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.UpdatedAt).HasDatabaseName("IX_Product_UpdatedAt");
        });

        // Configure Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.UpdatedAt).HasDatabaseName("IX_Order_UpdatedAt");
            // Implicit Many-to-Many
            entity.HasMany(e => e.Products)
                  .WithMany(e => e.Orders);
        });

        // Seed Data
        var random = new Random();
        var products = Enumerable.Range(1, 100).Select(i => new Product
        {
            Id = i,
            Name = $"Produto {i}",
            Price = Math.Round((decimal)(random.NextDouble() * 1000 + 50), 2) // preços entre 50 e 1050
        }).ToArray();

        modelBuilder.Entity<Product>().HasData(products);
    }
}
