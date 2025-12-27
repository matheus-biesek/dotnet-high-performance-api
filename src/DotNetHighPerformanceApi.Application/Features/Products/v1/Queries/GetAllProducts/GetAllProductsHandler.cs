using MediatR;
using DotNetHighPerformanceApi.Application.Caching;
using DotNetHighPerformanceApi.Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DotNetHighPerformanceApi.Application.Features.Products.v1.Queries.GetAllProducts;

public class GetAllProductsHandler(IAppDbContext context, IMapper mapper, ICacheService cache) : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IAppDbContext _context = context;
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
