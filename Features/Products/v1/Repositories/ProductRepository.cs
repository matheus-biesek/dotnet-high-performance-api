using DotNetHighPerformanceApi.Entities;
using DotNetHighPerformanceApi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DotNetHighPerformanceApi.Features.Products.v1.Repositories;

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
