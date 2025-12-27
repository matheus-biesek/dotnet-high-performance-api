using DotNetHighPerformanceApi.Features.Products.v1.DTOs;

namespace DotNetHighPerformanceApi.Features.Products.v1.Services;

public interface IProductService
{
    Task<(ProductDto? Data, string? ETag, bool IsNotModified)> GetByIdWithETagAsync(int id, string? ifNoneMatch = null, CancellationToken cancellationToken = default);
}
