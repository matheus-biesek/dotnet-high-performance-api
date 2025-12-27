using MediatR;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.Services;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Commands.CreateOrder;

public class CreateOrderHandler(IOrderService orderService) : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderService _orderService = orderService;

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.CreateOrderAsync(request.Payload, cancellationToken);
    }
}
