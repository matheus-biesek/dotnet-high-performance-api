using DotNetHighPerformanceApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetHighPerformanceApi.Persistence;

public class AppDbContext : DbContext
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
        });

        // Configure Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            // Implicit Many-to-Many
            entity.HasMany(e => e.Products)
                  .WithMany(e => e.Orders);
        });

        // Seed Data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Notebook Gamer", Price = 5000.00m },
            new Product { Id = 2, Name = "Mouse Sem Fio", Price = 150.00m },
            new Product { Id = 3, Name = "Teclado Mecânico", Price = 350.00m }
        );
    }
}
