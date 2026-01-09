using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces;

public interface IApiKeyService
{
    Task<bool> ValidateApiKeyAsync(string? apiKey);
}

