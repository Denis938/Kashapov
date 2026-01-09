using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Entities;

public class OrderProduct
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtOrder { get; set; }
    
    public int Quantity { get; set; } = 1;
    
    [ForeignKey("OrderId")]
    public Order Order { get; set; } = null!;
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}

