using ProductApi.Application.Models;

namespace ProductApi.Application.Abstractions;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateUpdateProductDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, CreateUpdateProductDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> FilterAsync(string categoria, decimal precioMin, CancellationToken ct = default);
}
