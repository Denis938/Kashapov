namespace WebApplication1.Models.DTO;

public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int EstimatedHours { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Medium";
    public int EstimatedHours { get; set; }
}

public class ProductUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Subject { get; set; }
    public string? Difficulty { get; set; }
    public int? EstimatedHours { get; set; }
    public bool? IsAvailable { get; set; }
}

