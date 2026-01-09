using WebApplication1.Models.DTO;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<OrderDto>> GetAllAsync(string userRole, int? userId = null)
    {
        if (userRole == "User" && userId.HasValue)
        {
            _logger.LogInformation("Получение заказов пользователя с ID: {UserId}", userId);
            var orders = await _orderRepository.GetByUserIdAsync(userId.Value);
            return orders.Select(MapToDto).ToList();
        }

        if (userRole == "Manager" || userRole == "Admin")
        {
            _logger.LogInformation("Получение всех заказов (роль: {Role})", userRole);
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto).ToList();
        }

        throw new UnauthorizedAccessException("Недостаточно прав для просмотра заказов");
    }

    public async Task<OrderDto?> GetByIdAsync(int id, string userRole, int? userId = null)
    {
        _logger.LogInformation("Получение заказа с ID: {OrderId}", id);
        var order = await _orderRepository.GetByIdAsync(id);
        
        if (order == null) return null;

        if (userRole == "User" && order.UserId != userId)
        {
            _logger.LogWarning("Попытка доступа к чужому заказу пользователем с ID: {UserId}", userId);
            throw new UnauthorizedAccessException("Нет доступа к этому заказу");
        }

        return MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto, int userId)
    {
        _logger.LogInformation("Создание заказа пользователем с ID: {UserId}", userId);

        if (dto.Items == null || !dto.Items.Any())
        {
            throw new ArgumentException("Заказ должен содержать хотя бы один продукт");
        }

        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = new List<Product>();
        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Продукт с ID {item.ProductId} не найден");
            }
            if (!product.IsAvailable)
            {
                throw new InvalidOperationException($"Продукт {product.Title} недоступен");
            }
            products.Add(product);
            totalAmount += product.Price * item.Quantity;
        }

        var order = new Order
        {
            UserId = userId,
            Status = "Pending",
            TotalAmount = totalAmount
        };

        var orderProducts = dto.Items.Select(item =>
        {
            var product = products.First(p => p.Id == item.ProductId);
            return new OrderProduct
            {
                ProductId = item.ProductId,
                PriceAtOrder = product.Price,
                Quantity = item.Quantity
            };
        }).ToList();

        var createdOrder = await _orderRepository.CreateWithProductsAsync(order, orderProducts);
        
        _logger.LogInformation("Заказ создан с ID: {OrderId}, сумма: {TotalAmount}", createdOrder.Id, totalAmount);
        return MapToDto(createdOrder);
    }

    public async Task<OrderDto> UpdateAsync(int id, OrderUpdateDto dto, string userRole, int? userId = null)
    {
        if (userRole != "Manager" && userRole != "Admin")
        {
            _logger.LogWarning("Попытка изменения заказа пользователем с ролью {Role}", userRole);
            throw new UnauthorizedAccessException("Недостаточно прав для изменения заказа");
        }

        _logger.LogInformation("Обновление заказа с ID: {OrderId}", id);

        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new KeyNotFoundException($"Заказ с ID {id} не найден");
        }

        if (dto.Status != null)
        {
            order.Status = dto.Status;
            if (dto.Status == "Completed")
            {
                order.CompletedAt = DateTime.UtcNow;
            }
        }

        var updated = await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Заказ обновлён с ID: {OrderId}", updated.Id);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, string userRole)
    {
        if (userRole != "Admin")
        {
            _logger.LogWarning("Попытка удаления заказа пользователем с ролью {Role}", userRole);
            throw new UnauthorizedAccessException("Недостаточно прав для удаления заказа");
        }

        _logger.LogInformation("Удаление заказа с ID: {OrderId}", id);
        var result = await _orderRepository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Заказ удалён с ID: {OrderId}", id);
        }
        return result;
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            CompletedAt = order.CompletedAt,
            Products = order.OrderProducts.Select(op => new OrderProductDto
            {
                ProductId = op.ProductId,
                ProductTitle = op.Product?.Title ?? "Unknown",
                PriceAtOrder = op.PriceAtOrder,
                Quantity = op.Quantity
            }).ToList()
        };
    }
}

