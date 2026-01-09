using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private const string CachePrefix = "product_";
    private const string CacheListPrefix = "products_list_";

    public ProductRepository(ApplicationDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var cacheKey = $"{CacheListPrefix}all";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<Product>>(cached) ?? new List<Product>();
        }

        var products = await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), 
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CachePrefix}{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Product>(cached);
        }

        var product = await _context.Products.FindAsync(id);
        
        if (product != null)
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        }

        return product;
    }

    public async Task<List<Product>> GetPagedAsync(int page, int pageSize, string? search = null, string? subject = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => 
                p.Title.Contains(search) || 
                (p.Description != null && p.Description.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            query = query.Where(p => p.Subject == subject);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? search = null, string? subject = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => 
                p.Title.Contains(search) || 
                (p.Description != null && p.Description.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            query = query.Where(p => p.Subject == subject);
        }

        return await query.CountAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await InvalidateCacheAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        await InvalidateCacheAsync(product.Id);
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        await InvalidateCacheAsync(id);
        return true;
    }

    private async Task InvalidateCacheAsync(int? productId = null)
    {
        await _cache.RemoveAsync($"{CacheListPrefix}all");
        
        if (productId.HasValue)
        {
            await _cache.RemoveAsync($"{CachePrefix}{productId.Value}");
        }
    }
}

