using Asp.Versioning;
using DotNetHighPerformanceApi.Domain.Entities;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace DotNetHighPerformanceApi.Features.Products.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/slow/products")]
public class SlowProductsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    // Bad Practice 1: No Caching
    public ActionResult<List<ProductDto>> GetAll()
    {
        // Bad Practice 2: Sync over Async (Blocking thread)
        // Bad Practice 3: No cancellation token support
        // Bad Practice 4: Fetching ALL data (No pagination)
        // Bad Practice 5: No AsNoTracking (Tracking overhead)
        
        var products = _context.Products.ToList(); // Sync call

        var result = new List<ProductDto>();

        foreach (var product in products)
        {
            // Bad Practice 6: N+1 Query Problem
            // Accessing a navigation property inside a loop triggers a separate SQL query for EACH product
            // because Lazy Loading is enabled.
            var orderCount = product.Orders.Count; 

            result.Add(new ProductDto(product.Id, product.Name, product.Price));
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    // Bad Practice: No Caching
    public ActionResult<ProductDto> GetById(int id)
    {
        // Bad Practice: Sync over Async
        var product = _context.Products.Find(id);

        if (product == null) return NotFound();

        return Ok(new ProductDto(product.Id, product.Name, product.Price));
    }
}
