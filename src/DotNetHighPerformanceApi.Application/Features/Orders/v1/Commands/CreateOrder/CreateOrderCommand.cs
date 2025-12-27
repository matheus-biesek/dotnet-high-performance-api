using MediatR;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto Payload) : IRequest<int>;
