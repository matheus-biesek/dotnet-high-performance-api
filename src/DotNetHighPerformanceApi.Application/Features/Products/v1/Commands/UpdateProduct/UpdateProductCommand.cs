using MediatR;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Products.v1.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<(ProductDto? Data, string? ETag, bool IsConflict)>
{
    public int Id { get; set; }
    public UpdateProductDto Dto { get; set; } = default!;
}
