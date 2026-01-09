using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Subject { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Difficulty { get; set; } = "Medium";
    
    public int EstimatedHours { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}

