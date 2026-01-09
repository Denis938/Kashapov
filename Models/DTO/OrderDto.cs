namespace WebApplication1.Models.DTO;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderProductDto> Products { get; set; } = new();
}

public class OrderProductDto
{
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public decimal PriceAtOrder { get; set; }
    public int Quantity { get; set; }
}

public class OrderCreateDto
{
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class OrderUpdateDto
{
    public string? Status { get; set; }
}

