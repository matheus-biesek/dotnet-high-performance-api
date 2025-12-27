using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Products.v1.Queries.GetProductById;

public record GetProductByIdQuery(int Id, string? IfNoneMatch = null) : IRequest<(ProductDto? Data, string? ETag, bool IsNotModified)>;
