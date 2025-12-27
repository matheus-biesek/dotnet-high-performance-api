using AutoMapper;
using AutoMapper.QueryableExtensions;
using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DotNetHighPerformanceApi.Caching;
using System.Text.Json;

namespace DotNetHighPerformanceApi.Features.Products.v1.Queries.GetProductById;

public class GetProductByIdHandler(AppDbContext context, IMapper mapper, ICacheService cache) : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cache = cache;

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"product-{request.Id}";

        var cached = await _cache.GetAsync<ProductDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (product != null)
        {
            await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(5), cancellationToken);
        }

        return product;
    }
}
