using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DotNetHighPerformanceApi.Caching;
using System.Text.Json;

namespace DotNetHighPerformanceApi.Features.Products.v1.Queries.GetAllProducts;

public class GetAllProductsHandler(AppDbContext context, IMapper mapper, ICacheService cache) : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cache = cache;

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "products-all";
        
        var cached = await _cache.GetAsync<List<ProductDto>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var products = await _context.Products
            .AsNoTracking()
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5), cancellationToken);

        return products;
    }
}
