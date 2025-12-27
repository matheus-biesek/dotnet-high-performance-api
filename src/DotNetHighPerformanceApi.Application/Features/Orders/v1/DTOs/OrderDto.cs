using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;

namespace DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;

public record OrderDto(int Id, DateTime CreatedAt, decimal Total, List<ProductDto> Products);
