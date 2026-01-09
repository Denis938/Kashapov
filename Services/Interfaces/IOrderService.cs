using WebApplication1.Models.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync(string userRole, int? userId = null);
    Task<OrderDto?> GetByIdAsync(int id, string userRole, int? userId = null);
    Task<OrderDto> CreateAsync(OrderCreateDto dto, int userId);
    Task<OrderDto> UpdateAsync(int id, OrderUpdateDto dto, string userRole, int? userId = null);
    Task<bool> DeleteAsync(int id, string userRole);
}

