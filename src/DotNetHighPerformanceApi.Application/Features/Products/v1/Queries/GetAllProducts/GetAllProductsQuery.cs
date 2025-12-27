using MediatR;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Products.v1.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;
