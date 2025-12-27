using MediatR;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Products.v1.Queries.GetProductById;

public record GetProductByIdQuery(int Id, string? IfNoneMatch = null) : IRequest<(ProductDto? Data, string? ETag, bool IsNotModified)>;
