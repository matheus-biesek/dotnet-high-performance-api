using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Services;

public interface IOrderService
{
    Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<(OrderDto? Data, string? ETag, bool IsNotModified)> GetByIdWithETagAsync(int id, string? ifNoneMatch = null, CancellationToken cancellationToken = default);
}
