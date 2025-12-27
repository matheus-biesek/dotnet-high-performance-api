using AutoMapper;
using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Features.Products.v1.DTOs;
using DotNetHighPerformanceApi.Entities;

namespace DotNetHighPerformanceApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Order, OrderDto>();
    }
}
