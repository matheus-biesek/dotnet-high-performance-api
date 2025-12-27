using AutoMapper;
using DotNetHighPerformanceApi.Caching;
using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotNetHighPerformanceApi.Features.Products.v1.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    AppDbContext context,
    IMapper mapper,
    IETagService eTagService,
    ICacheService cacheService)
    : IRequestHandler<UpdateProductCommand, (ProductDto? Data, string? ETag, bool IsConflict)>
{
    public async Task<(ProductDto? Data, string? ETag, bool IsConflict)> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Fetch product with current version
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (product == null)
            return (null, null, false); // Not found will be handled by controller

        // Check for version conflict (optimistic locking)
        if (product.Version != request.Dto.Version)
            return (null, null, true); // Conflict - version mismatch

        // Update product
        product.Name = request.Dto.Name;
        product.Price = request.Dto.Price;
        product.Version++; // Increment version for next ETag
        product.UpdatedAt = DateTime.UtcNow;

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, null, true); // Concurrent modification detected
        }

        // Invalidate cache
        await cacheService.RemoveAsync($"product-{request.Id}", cancellationToken);

        // Map and return with new ETag
        var dto = mapper.Map<ProductDto>(product);
        var newETag = eTagService.GenerateETag("product", request.Id, product.Version);

        return (dto, newETag, false);
    }
}
