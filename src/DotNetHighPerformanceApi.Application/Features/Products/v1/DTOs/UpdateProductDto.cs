namespace DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;

public class UpdateProductDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int Version { get; set; } // Required for If-Match validation
}
