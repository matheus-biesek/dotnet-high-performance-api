using MediatR;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id, string? IfNoneMatch = null) : IRequest<(OrderDto? Data, string? ETag, bool IsNotModified)>;
