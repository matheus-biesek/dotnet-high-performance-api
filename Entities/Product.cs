namespace DotNetHighPerformanceApi.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1; 
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
