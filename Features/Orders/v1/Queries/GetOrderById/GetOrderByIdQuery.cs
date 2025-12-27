using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;
