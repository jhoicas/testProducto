using ProductApi.Application.Abstractions;
using ProductApi.Application.Models;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<ProductDto> CreateAsync(CreateUpdateProductDto dto, CancellationToken ct = default)
    {
        Validate(dto);
        var entity = new Product
        {
            Nombre = dto.Nombre.Trim(),
            Precio = dto.Precio,
            Categoria = dto.Categoria.Trim(),
            Stock = dto.Stock
        };
        var created = await _repo.AddAsync(entity, ct);
        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, CreateUpdateProductDto dto, CancellationToken ct = default)
    {
        Validate(dto);
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return false;
        existing.Nombre = dto.Nombre.Trim();
        existing.Precio = dto.Precio;
        existing.Categoria = dto.Categoria.Trim();
        existing.Stock = dto.Stock;
        return await _repo.UpdateAsync(existing, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    public async Task<IReadOnlyList<ProductDto>> FilterAsync(string categoria, decimal precioMin, CancellationToken ct = default)
    {
        categoria = categoria?.Trim() ?? string.Empty;
        if (precioMin < 0) throw new ArgumentException("El precio mínimo no puede ser negativo", nameof(precioMin));
        var items = await _repo.FilterAsync(categoria, precioMin, ct);
        return items.Select(MapToDto).ToList();
    }

    private static void Validate(CreateUpdateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(dto.Nombre));
        if (dto.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0", nameof(dto.Precio));
        if (dto.Stock < 0)
            throw new ArgumentException("El stock no puede ser negativo", nameof(dto.Stock));
        if (string.IsNullOrWhiteSpace(dto.Categoria))
            throw new ArgumentException("La categoría no puede estar vacía", nameof(dto.Categoria));
    }

    private static ProductDto MapToDto(Product p) => new(p.Id, p.Nombre, p.Precio, p.Categoria, p.Stock);
}
