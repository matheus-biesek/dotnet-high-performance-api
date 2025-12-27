namespace DotNetHighPerformanceApi.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    // For Many-to-Many relationship with Orders (Implicit)
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
