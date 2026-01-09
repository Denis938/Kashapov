using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;
using Moq;

namespace WebApplication1.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _cacheMock = new Mock<IDistributedCache>();
        _repository = new ProductRepository(_context, _cacheMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct()
    {
        var product = new Product
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 100.50m,
            Subject = "Math",
            Difficulty = "Medium",
            EstimatedHours = 5,
            IsAvailable = true
        };

        var result = await _repository.CreateAsync(product);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Product");
        
        var fromDb = await _context.Products.FindAsync(result.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Title.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        var product = new Product
        {
            Title = "Test Product",
            Price = 100m,
            Subject = "Math",
            EstimatedHours = 5
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(product.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Title.Should().Be("Test Product");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductNotFound()
    {
        var result = await _repository.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        var product = new Product
        {
            Title = "Original Title",
            Price = 100m,
            Subject = "Math",
            EstimatedHours = 5
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        product.Title = "Updated Title";
        product.Price = 200m;

        var result = await _repository.UpdateAsync(product);

        result.Title.Should().Be("Updated Title");
        result.Price.Should().Be(200m);
        
        var fromDb = await _context.Products.FindAsync(product.Id);
        fromDb!.Title.Should().Be("Updated Title");
        fromDb.Price.Should().Be(200m);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct()
    {
        var product = new Product
        {
            Title = "To Delete",
            Price = 100m,
            Subject = "Math",
            EstimatedHours = 5
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        var productId = product.Id;

        var result = await _repository.DeleteAsync(productId);

        result.Should().BeTrue();
        var fromDb = await _context.Products.FindAsync(productId);
        fromDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound()
    {
        var result = await _repository.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResults()
    {
        for (int i = 1; i <= 15; i++)
        {
            _context.Products.Add(new Product
            {
                Title = $"Product {i}",
                Price = 100m,
                Subject = "Math",
                EstimatedHours = 5
            });
        }
        await _context.SaveChangesAsync();

        var result = await _repository.GetPagedAsync(page: 1, pageSize: 10);

        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterBySearch()
    {
        _context.Products.Add(new Product { Title = "Math Homework", Price = 100m, Subject = "Math", EstimatedHours = 5 });
        _context.Products.Add(new Product { Title = "Physics Task", Price = 100m, Subject = "Physics", EstimatedHours = 5 });
        await _context.SaveChangesAsync();

        var result = await _repository.GetPagedAsync(page: 1, pageSize: 10, search: "Math");

        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("Math");
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCorrectCount()
    {
        for (int i = 1; i <= 5; i++)
        {
            _context.Products.Add(new Product
            {
                Title = $"Product {i}",
                Price = 100m,
                Subject = "Math",
                EstimatedHours = 5
            });
        }
        await _context.SaveChangesAsync();

        var result = await _repository.GetTotalCountAsync();

        result.Should().Be(5);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

