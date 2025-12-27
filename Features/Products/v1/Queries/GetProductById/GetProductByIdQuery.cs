using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Products.v1.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
