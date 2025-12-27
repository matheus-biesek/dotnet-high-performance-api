using DotNetHighPerformanceApi.Application.Caching;
using AutoMapper;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Domain.Repositories;
using DotNetHighPerformanceApi.Application.Common.Interfaces;

namespace DotNetHighPerformanceApi.Application.Features.Products.v1.Services;

public class ProductService(IProductRepository productRepository, IMapper mapper, ICacheService cache, IETagService eTagService) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cache = cache;
    private readonly IETagService _eTagService = eTagService;

    public async Task<(ProductDto? Data, string? ETag, bool IsNotModified)> GetByIdWithETagAsync(int id, string? ifNoneMatch = null, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"product-{id}";
        var cached = await _cache.GetAsync<(ProductDto, int)>(cacheKey, cancellationToken);
        if (cached.Item1 != null)
        {
            var eTag = _eTagService.GenerateETag("product", id, cached.Item2);
            if (ifNoneMatch?.Equals(eTag) == true)
                return (null, eTag, true);
            return (cached.Item1, eTag, false);
        }

        var products = await _productRepository.GetByIdsAsync(new[] { id }, cancellationToken);
        if (products.Count == 0)
            return (null, null, false);

        var product = products.First();
        var productDto = _mapper.Map<ProductDto>(product);
        var eTagValue = _eTagService.GenerateETag("product", id, product.Version);

        // Cache with Version for ETag generation
        await _cache.SetAsync(cacheKey, (productDto, product.Version), TimeSpan.FromMinutes(5), cancellationToken);

        if (ifNoneMatch?.Equals(eTagValue) == true)
            return (null, eTagValue, true);

        return (productDto, eTagValue, false);
    }
}
