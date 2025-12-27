using AutoMapper;
using DotNetHighPerformanceApi.Entities;
using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Features.Orders.v1.Repositories;
using DotNetHighPerformanceApi.Features.Products.v1.Repositories;
using DotNetHighPerformanceApi.Caching;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Services;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper, ICacheService cache) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cache = cache;

    public async Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var productIds = dto.ProductIds.Distinct().ToList();

        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new Exception("Um ou mais produtos não foram encontrados.");
        }

        var order = new Order
        {
            CreatedAt = DateTime.UtcNow,
            Products = products,
            Total = products.Sum(p => p.Price)
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"order-{id}";
        var cached = await _cache.GetAsync<OrderDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null) return null;

        var dto = _mapper.Map<OrderDto>(order);

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);

        return dto;
    }
}
