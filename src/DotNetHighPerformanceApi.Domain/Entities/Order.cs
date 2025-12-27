namespace DotNetHighPerformanceApi.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public decimal Total { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
