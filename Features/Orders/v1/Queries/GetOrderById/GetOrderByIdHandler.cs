using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Features.Orders.v1.Services;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Queries.GetOrderById;

public class GetOrderByIdHandler(IOrderService orderService) : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderService _orderService = orderService;

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetByIdAsync(request.Id, cancellationToken);
    }
}
