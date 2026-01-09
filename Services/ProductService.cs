using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        _logger.LogInformation("Получение списка всех продуктов");
        var products = await _repository.GetAllAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Получение продукта с ID: {ProductId}", id);
        var product = await _repository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<PagedResponseDto<ProductDto>> GetPagedAsync(int page, int pageSize, string? search = null, string? subject = null)
    {
        _logger.LogInformation("Получение продуктов с пагинацией: страница {Page}, размер {PageSize}, поиск: {Search}, предмет: {Subject}", 
            page, pageSize, search, subject);
        
        var products = await _repository.GetPagedAsync(page, pageSize, search, subject);
        var total = await _repository.GetTotalCountAsync(search, subject);

        return new PagedResponseDto<ProductDto>
        {
            Items = products.Select(MapToDto).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto, string userRole)
    {
        if (userRole != "Admin" && userRole != "Manager")
        {
            _logger.LogWarning("Попытка создания продукта пользователем с ролью {Role}", userRole);
            throw new UnauthorizedAccessException("Недостаточно прав для создания продукта");
        }

        _logger.LogInformation("Создание нового продукта: {Title}", dto.Title);

        var product = new Product
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Subject = dto.Subject,
            Difficulty = dto.Difficulty,
            EstimatedHours = dto.EstimatedHours,
            IsAvailable = true
        };

        var created = await _repository.CreateAsync(product);
        _logger.LogInformation("Продукт создан с ID: {ProductId}", created.Id);
        return MapToDto(created);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductUpdateDto dto, string userRole)
    {
        if (userRole != "Admin" && userRole != "Manager")
        {
            _logger.LogWarning("Попытка изменения продукта пользователем с ролью {Role}", userRole);
            throw new UnauthorizedAccessException("Недостаточно прав для изменения продукта");
        }

        _logger.LogInformation("Обновление продукта с ID: {ProductId}", id);

        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Продукт с ID {id} не найден");
        }

        if (dto.Title != null) product.Title = dto.Title;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Subject != null) product.Subject = dto.Subject;
        if (dto.Difficulty != null) product.Difficulty = dto.Difficulty;
        if (dto.EstimatedHours.HasValue) product.EstimatedHours = dto.EstimatedHours.Value;
        if (dto.IsAvailable.HasValue) product.IsAvailable = dto.IsAvailable.Value;

        var updated = await _repository.UpdateAsync(product);
        _logger.LogInformation("Продукт обновлён с ID: {ProductId}", updated.Id);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, string userRole)
    {
        if (userRole != "Admin")
        {
            _logger.LogWarning("Попытка удаления продукта пользователем с ролью {Role}", userRole);
            throw new UnauthorizedAccessException("Недостаточно прав для удаления продукта");
        }

        _logger.LogInformation("Удаление продукта с ID: {ProductId}", id);
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Продукт удалён с ID: {ProductId}", id);
        }
        return result;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            Subject = product.Subject,
            Difficulty = product.Difficulty,
            EstimatedHours = product.EstimatedHours,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt
        };
    }
}

