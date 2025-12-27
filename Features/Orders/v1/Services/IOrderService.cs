using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Services;

public interface IOrderService
{
    Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
