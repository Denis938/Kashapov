using WebApplication1.Models.Entities;

namespace WebApplication1.Repositories.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByKeyAsync(string key);
    Task<ApiKey> CreateAsync(ApiKey apiKey);
    Task<List<ApiKey>> GetAllAsync();
}

