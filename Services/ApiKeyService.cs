using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(IApiKeyRepository repository, ILogger<ApiKeyService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> ValidateApiKeyAsync(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        var key = await _repository.GetByKeyAsync(apiKey);
        
        if (key == null)
        {
            _logger.LogWarning("Попытка использования несуществующего API ключа");
            return false;
        }

        if (!key.IsActive)
        {
            _logger.LogWarning("Попытка использования неактивного API ключа: {Key}", apiKey);
            return false;
        }

        if (key.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Попытка использования истёкшего API ключа: {Key}", apiKey);
            return false;
        }

        _logger.LogInformation("API ключ успешно валидирован");
        return true;
    }
}

