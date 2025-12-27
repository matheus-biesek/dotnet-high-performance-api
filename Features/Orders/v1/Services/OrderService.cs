using AutoMapper;
using DotNetHighPerformanceApi.Entities;
using DotNetHighPerformanceApi.Features.Orders.v1.DTOs;
using DotNetHighPerformanceApi.Features.Orders.v1.Repositories;
using DotNetHighPerformanceApi.Features.Products.v1.Repositories;
using DotNetHighPerformanceApi.Caching;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Services;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper, ICacheService cache, IETagService eTagService) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cache = cache;
    private readonly IETagService _eTagService = eTagService;

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

    public async Task<(OrderDto? Data, string? ETag, bool IsNotModified)> GetByIdWithETagAsync(int id, string? ifNoneMatch = null, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"order-{id}";
        var cached = await _cache.GetAsync<(OrderDto, int)>(cacheKey, cancellationToken);
        if (cached.Item1 != null)
        {
            var eTag = _eTagService.GenerateETag("order", id, cached.Item2);
            if (ifNoneMatch?.Equals(eTag) == true)
                return (null, eTag, true);
            return (cached.Item1, eTag, false);
        }

        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return (null, null, false);

        var dto = _mapper.Map<OrderDto>(order);
        var eTagValue = _eTagService.GenerateETag("order", id, order.Version);

        // Cache with Version for ETag generation
        await _cache.SetAsync(cacheKey, (dto, order.Version), TimeSpan.FromMinutes(10), cancellationToken);

        if (ifNoneMatch?.Equals(eTagValue) == true)
            return (null, eTagValue, true);

        return (dto, eTagValue, false);
    }
}
