using DotNetHighPerformanceApi.Entities;

namespace DotNetHighPerformanceApi.Features.Products.v1.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
