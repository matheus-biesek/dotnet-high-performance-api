using DotNetHighPerformanceApi.Features.Products.v1.DTOs;

namespace DotNetHighPerformanceApi.Features.Products.v1.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
