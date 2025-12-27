using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Products.v1.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<(ProductDto? Data, string? ETag, bool IsConflict)>
{
    public int Id { get; set; }
    public UpdateProductDto Dto { get; set; } = default!;
}
