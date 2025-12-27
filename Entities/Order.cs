namespace DotNetHighPerformanceApi.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal Total { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
