using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using WebApplication1.Data;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using Dapper;

namespace WebApplication1.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _dbConnection;

    public OrderRepository(ApplicationDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Order> CreateWithProductsAsync(Order order, List<OrderProduct> orderProducts)
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            _dbConnection.Open();
        }

        using var transaction = _dbConnection.BeginTransaction();
        try
        {
            var orderSql = @"
                INSERT INTO ""Orders"" (""UserId"", ""Status"", ""TotalAmount"", ""CreatedAt"")
                VALUES (@UserId, @Status, @TotalAmount, @CreatedAt)
                RETURNING ""Id"", ""UserId"", ""Status"", ""TotalAmount"", ""CreatedAt"", ""CompletedAt""";

            var orderParams = new
            {
                order.UserId,
                order.Status,
                order.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            var createdOrder = await _dbConnection.QueryFirstOrDefaultAsync<Order>(
                orderSql, orderParams, transaction);

            if (createdOrder == null)
            {
                throw new Exception("Не удалось создать заказ");
            }

            foreach (var orderProduct in orderProducts)
            {
                var orderProductSql = @"
                    INSERT INTO ""OrderProducts"" (""OrderId"", ""ProductId"", ""PriceAtOrder"", ""Quantity"")
                    VALUES (@OrderId, @ProductId, @PriceAtOrder, @Quantity)
                    RETURNING ""Id"", ""OrderId"", ""ProductId"", ""PriceAtOrder"", ""Quantity""";

                var orderProductParams = new
                {
                    OrderId = createdOrder.Id,
                    orderProduct.ProductId,
                    orderProduct.PriceAtOrder,
                    orderProduct.Quantity
                };

                await _dbConnection.ExecuteAsync(
                    orderProductSql, orderProductParams, transaction);
            }

            transaction.Commit();

            return await GetByIdAsync(createdOrder.Id) ?? createdOrder;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}

