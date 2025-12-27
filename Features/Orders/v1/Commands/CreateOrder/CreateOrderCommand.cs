using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto Payload) : IRequest<int>;
