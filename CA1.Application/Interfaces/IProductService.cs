using CA1.Application.DTOs;

namespace CA1.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task CreateAsync(ProductDto productDto);
    Task UpdateAsync(int id, ProductDto productDto);
    Task DeleteAsync(int id);
}
