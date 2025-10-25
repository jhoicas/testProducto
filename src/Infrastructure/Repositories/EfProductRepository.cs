using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Abstractions;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

public class EfProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public EfProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => await _db.Productos.AsNoTracking().OrderBy(p => p.Id).ToListAsync(ct);

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Productos.FindAsync(new object?[] { id }, ct);

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        _db.Productos.Add(product);
        await _db.SaveChangesAsync(ct);
        return product;
    }

    public async Task<bool> UpdateAsync(Product product, CancellationToken ct = default)
    {
        var exists = await _db.Productos.AnyAsync(p => p.Id == product.Id, ct);
        if (!exists) return false;
        _db.Productos.Update(product);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Productos.FindAsync(new object?[] { id }, ct);
        if (entity is null) return false;
        _db.Productos.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<Product>> FilterAsync(string categoria, decimal precioMin, CancellationToken ct = default)
    {
        var q = _db.Productos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(categoria))
            q = q.Where(p => p.Categoria.ToLower() == categoria.ToLower());
        q = q.Where(p => p.Precio >= precioMin);
        return await q.OrderBy(p => p.Id).ToListAsync(ct);
    }
}
