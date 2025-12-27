using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using MediatR;

namespace DotNetHighPerformanceApi.Features.Products.v1.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;
