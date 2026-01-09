using WebApplication1.Models.DTO;

namespace WebApplication1.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<PagedResponseDto<ProductDto>> GetPagedAsync(int page, int pageSize, string? search = null, string? subject = null);
    Task<ProductDto> CreateAsync(ProductCreateDto dto, string userRole);
    Task<ProductDto> UpdateAsync(int id, ProductUpdateDto dto, string userRole);
    Task<bool> DeleteAsync(int id, string userRole);
}

