using DotNetHighPerformanceApi.Features.Orders.v1.Commands.CreateOrder;
using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Features.Orders.v1.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotNetHighPerformanceApi.Features.Orders.v1;

[ApiController]
[Route("orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateOrderDto dto)
    {
        var command = new CreateOrderCommand(dto);
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = orderId }, orderId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }
}
