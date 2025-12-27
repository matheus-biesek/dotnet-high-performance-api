using DotNetHighPerformanceApi.Domain.Entities;
using DotNetHighPerformanceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DotNetHighPerformanceApi.Domain.Repositories;

namespace DotNetHighPerformanceApi.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        return await _context.Products
            .Where(p => idList.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}
