using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetPagedAsync(int page, int pageSize, string? search = null, string? subject = null);
    Task<int> GetTotalCountAsync(string? search = null, string? subject = null);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

