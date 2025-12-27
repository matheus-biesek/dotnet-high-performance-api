using DotNetHighPerformanceApi.Features.Orders.v1.Commands.CreateOrder;
using DotNetHighPerformanceApi.Features.Orders.v1.Commands.UpdateOrder;
using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Features.Orders.v1.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace DotNetHighPerformanceApi.Features.Orders.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/orders")]
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
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Encoding")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var ifNoneMatch = Request.Headers.IfNoneMatch.ToString();
        var (data, eTag, isNotModified) = await _mediator.Send(new GetOrderByIdQuery(id, ifNoneMatch));
        
        if (isNotModified)
        {
            Response.Headers.ETag = eTag!;
            return StatusCode(304); // Not Modified
        }

        if (data == null) return NotFound();
        
        if (!string.IsNullOrEmpty(eTag))
            Response.Headers.ETag = eTag;
        
        return Ok(data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] UpdateOrderDto dto)
    {
        var ifMatch = Request.Headers.IfMatch.ToString();
        if (string.IsNullOrEmpty(ifMatch))
            return BadRequest("If-Match header is required for PUT operations");

        var (data, eTag, isConflict) = await _mediator.Send(new UpdateOrderCommand
        {
            Id = id,
            Dto = dto
        });

        if (isConflict)
            return StatusCode(412); // Precondition Failed

        if (data == null)
            return NotFound();

        if (!string.IsNullOrEmpty(eTag))
            Response.Headers.ETag = eTag;

        return Ok(data);
    }
}
