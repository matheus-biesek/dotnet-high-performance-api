using DotNetHighPerformanceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetHighPerformanceApi.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
