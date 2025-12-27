using AutoMapper;
using DotNetHighPerformanceApi.Application.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Application.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Domain.Entities;

namespace DotNetHighPerformanceApi.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Order, OrderDto>();
    }
}
