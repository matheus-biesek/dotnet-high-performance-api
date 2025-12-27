using MediatR;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<(OrderDto? Data, string? ETag, bool IsConflict)>
{
    public int Id { get; set; }
    public UpdateOrderDto Dto { get; set; } = default!;
}
