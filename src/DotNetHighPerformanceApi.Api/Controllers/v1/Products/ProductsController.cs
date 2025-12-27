using DotNetHighPerformanceApi.Application.Features.Products.v1.Commands.UpdateProduct;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Application.Features.Products.v1.Queries.GetAllProducts;
using DotNetHighPerformanceApi.Application.Features.Products.v1.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using Asp.Versioning;

namespace DotNetHighPerformanceApi.Features.Products.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/products")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Encoding")]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllProductsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByHeader = "Accept-Encoding")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var ifNoneMatch = Request.Headers.IfNoneMatch.ToString();
        var (data, eTag, isNotModified) = await _mediator.Send(new GetProductByIdQuery(id, ifNoneMatch));
        
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
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var ifMatch = Request.Headers.IfMatch.ToString();
        if (string.IsNullOrEmpty(ifMatch))
            return BadRequest("If-Match header is required for PUT operations");

        var (data, eTag, isConflict) = await _mediator.Send(new UpdateProductCommand
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
