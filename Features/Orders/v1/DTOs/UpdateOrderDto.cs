namespace DotNetHighPerformanceApi.Features.Orders.v1.DTOs;

public class UpdateOrderDto
{
    public int Version { get; set; } // Required for If-Match validation
}
