using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Features.Products.v1.Services;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Products.v1.Queries.GetProductById;

public class GetProductByIdHandler(IProductService productService) : IRequestHandler<GetProductByIdQuery, (ProductDto? Data, string? ETag, bool IsNotModified)>
{
    private readonly IProductService _productService = productService;

    public async Task<(ProductDto? Data, string? ETag, bool IsNotModified)> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetByIdWithETagAsync(request.Id, request.IfNoneMatch, cancellationToken);
    }
}
