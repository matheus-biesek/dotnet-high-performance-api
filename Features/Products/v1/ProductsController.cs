using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Features.Products.v1.Queries.GetAllProducts;
using DotNetHighPerformanceApi.Features.Products.v1.Queries.GetProductById;
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
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }
}
