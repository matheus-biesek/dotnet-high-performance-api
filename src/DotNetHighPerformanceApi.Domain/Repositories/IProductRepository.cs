using DotNetHighPerformanceApi.Domain.Entities;

namespace DotNetHighPerformanceApi.Domain.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
