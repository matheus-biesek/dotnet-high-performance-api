using MediatR;
using DotNetHighPerformanceApi.Application.Caching;
using DotNetHighPerformanceApi.Application.Common.Interfaces;
using AutoMapper;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(
    IAppDbContext context,
    IMapper mapper,
    IETagService eTagService,
    ICacheService cacheService)
    : IRequestHandler<UpdateOrderCommand, (OrderDto? Data, string? ETag, bool IsConflict)>
{
    public async Task<(OrderDto? Data, string? ETag, bool IsConflict)> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        // Fetch order with current version
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        
        if (order == null)
            return (null, null, false); // Not found will be handled by controller

        // Check for version conflict (optimistic locking)
        if (order.Version != request.Dto.Version)
            return (null, null, true); // Conflict - version mismatch

        // Update order version (demonstrating version increment)
        order.Version++; 
        order.UpdatedAt = DateTime.UtcNow;

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, null, true); // Concurrent modification detected
        }

        // Invalidate cache
        await cacheService.RemoveAsync($"order-{request.Id}", cancellationToken);

        // Map and return with new ETag
        var dto = mapper.Map<OrderDto>(order);
        var newETag = eTagService.GenerateETag("order", request.Id, order.Version);

        return (dto, newETag, false);
    }
}
