using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<(OrderDto? Data, string? ETag, bool IsConflict)>
{
    public int Id { get; set; }
    public UpdateOrderDto Dto { get; set; } = default!;
}
