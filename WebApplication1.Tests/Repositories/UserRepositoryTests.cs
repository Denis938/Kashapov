using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using Xunit;

namespace WebApplication1.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hashed_password",
            Role = "User"
        };

        var result = await _repository.CreateAsync(user);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@test.com");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var fromDb = await _context.Users.FindAsync(result.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hashed_password",
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var result = await _repository.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hashed_password",
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByUsernameAsync("testuser");

        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var result = await _repository.GetByUsernameAsync("nonexistent");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hashed_password",
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync("test@test.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("test@test.com");
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var result = await _repository.GetByEmailAsync("nonexistent@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        var user = new User
        {
            Username = "originaluser",
            Email = "original@test.com",
            PasswordHash = "hashed_password",
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.Username = "updateduser";
        user.Email = "updated@test.com";
        user.Role = "Manager";

        var result = await _repository.UpdateAsync(user);

        result.Username.Should().Be("updateduser");
        result.Email.Should().Be("updated@test.com");
        result.Role.Should().Be("Manager");
        
        var fromDb = await _context.Users.FindAsync(user.Id);
        fromDb!.Username.Should().Be("updateduser");
        fromDb.Email.Should().Be("updated@test.com");
        fromDb.Role.Should().Be("Manager");
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAt()
    {
        var user = new User
        {
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hashed_password",
            Role = "User"
        };

        var result = await _repository.CreateAsync(user);

        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

