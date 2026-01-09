using WebApplication1.Services.Interfaces;

namespace WebApplication1.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.StartsWith("/api/auth") || 
            path.StartsWith("/health") || 
            path.StartsWith("/swagger") ||
            path.StartsWith("/metrics"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            await _next(context);
            return;
        }

        var apiKey = context.Request.Headers["X-API-KEY"].ToString();
        if (!string.IsNullOrEmpty(apiKey))
        {
            var isValid = await apiKeyService.ValidateApiKeyAsync(apiKey);
            if (isValid)
            {
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim("apiKey", apiKey),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "ApiClient")
                };
                var identity = new System.Security.Claims.ClaimsIdentity(claims, "ApiKey");
                context.User = new System.Security.Claims.ClaimsPrincipal(identity);
                
                await _next(context);
                return;
            }
            else
            {
                _logger.LogWarning("Невалидный или истёкший API ключ");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or expired API key");
                return;
            }
        }

        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Authentication required. Provide JWT Bearer token or X-API-KEY header.");
    }
}

