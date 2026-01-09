using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests.Repositories;

public class ApiKeyRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ApiKeyRepository _repository;

    public ApiKeyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ApiKeyRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateApiKey()
    {
        var apiKey = new ApiKey
        {
            Key = "test-api-key-12345",
            Description = "Test API Key",
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        var result = await _repository.CreateAsync(apiKey);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Key.Should().Be("test-api-key-12345");
        result.Description.Should().Be("Test API Key");
        result.IsActive.Should().BeTrue();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var fromDb = await _context.ApiKeys.FindAsync(result.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Key.Should().Be("test-api-key-12345");
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldReturnApiKey()
    {
        var apiKey = new ApiKey
        {
            Key = "test-api-key-12345",
            Description = "Test API Key",
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByKeyAsync("test-api-key-12345");

        result.Should().NotBeNull();
        result!.Key.Should().Be("test-api-key-12345");
        result.Id.Should().Be(apiKey.Id);
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenApiKeyNotFound()
    {
        var result = await _repository.GetByKeyAsync("nonexistent-key");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllApiKeys()
    {
        var apiKey1 = new ApiKey
        {
            Key = "key-1",
            Description = "Key 1",
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        var apiKey2 = new ApiKey
        {
            Key = "key-2",
            Description = "Key 2",
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _context.ApiKeys.AddRange(apiKey1, apiKey2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(k => k.Key == "key-1");
        result.Should().Contain(k => k.Key == "key-2");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoApiKeys()
    {
        var result = await _repository.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAt()
    {
        var apiKey = new ApiKey
        {
            Key = "test-api-key",
            Description = "Test",
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        var result = await _repository.CreateAsync(apiKey);

        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetByKeyAsync_ShouldBeCaseSensitive()
    {
        var apiKey = new ApiKey
        {
            Key = "Test-API-Key",
            Description = "Test",
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _context.ApiKeys.Add(apiKey);
        await _context.SaveChangesAsync();

        var result1 = await _repository.GetByKeyAsync("Test-API-Key");
        var result2 = await _repository.GetByKeyAsync("test-api-key");

        result1.Should().NotBeNull();
        result2.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

