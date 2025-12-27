using MediatR;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.Services;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Queries.GetOrderById;

public class GetOrderByIdHandler(IOrderService orderService) : IRequestHandler<GetOrderByIdQuery, (OrderDto? Data, string? ETag, bool IsNotModified)>
{
    private readonly IOrderService _orderService = orderService;

    public async Task<(OrderDto? Data, string? ETag, bool IsNotModified)> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetByIdWithETagAsync(request.Id, request.IfNoneMatch, cancellationToken);
    }
}
