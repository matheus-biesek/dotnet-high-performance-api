using AutoMapper;
using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Features.Products.v1.Repositories;

namespace DotNetHighPerformanceApi.Features.Products.v1.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetByIdsAsync(ids, cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }
}
