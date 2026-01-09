using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;
using Moq;
using System.Data;

namespace WebApplication1.Tests.Repositories;

public class OrderRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IDbConnection> _dbConnectionMock;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _dbConnectionMock = new Mock<IDbConnection>();
        _repository = new OrderRepository(_context, _dbConnectionMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateOrder()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            Status = "Pending",
            TotalAmount = 100m
        };

        var result = await _repository.CreateAsync(order);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.UserId.Should().Be(user.Id);
        
        var fromDb = await _context.Orders.FindAsync(result.Id);
        fromDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            Status = "Pending",
            TotalAmount = 100m
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(order.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserOrders()
    {
        var user1 = new User { Username = "user1", Email = "user1@test.com", PasswordHash = "hash", Role = "User" };
        var user2 = new User { Username = "user2", Email = "user2@test.com", PasswordHash = "hash", Role = "User" };
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var order1 = new Order { UserId = user1.Id, Status = "Pending", TotalAmount = 100m };
        var order2 = new Order { UserId = user1.Id, Status = "Completed", TotalAmount = 200m };
        var order3 = new Order { UserId = user2.Id, Status = "Pending", TotalAmount = 150m };
        _context.Orders.AddRange(order1, order2, order3);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUserIdAsync(user1.Id);

        result.Should().HaveCount(2);
        result.All(o => o.UserId == user1.Id).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrder()
    {
        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "hash", Role = "User" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            Status = "Pending",
            TotalAmount = 100m
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        order.Status = "Completed";
        order.CompletedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(order);

        result.Status.Should().Be("Completed");
        result.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteOrder()
    {
        var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "hash", Role = "User" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var order = new Order
        {
            UserId = user.Id,
            Status = "Pending",
            TotalAmount = 100m
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        var orderId = order.Id;

        var result = await _repository.DeleteAsync(orderId);

        result.Should().BeTrue();
        var fromDb = await _context.Orders.FindAsync(orderId);
        fromDb.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

