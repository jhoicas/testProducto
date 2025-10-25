using System.Collections.Concurrent;
using System.Threading;
using ProductApi.Application.Abstractions;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _store = new();
    private int _idSeq = 0;

    public InMemoryProductRepository()
    {
        // datos semillas
        AddAsync(new Product { Nombre = "Teclado", Precio = 25.99m, Categoria = "Perifericos", Stock = 100 }).GetAwaiter().GetResult();
        AddAsync(new Product { Nombre = "Mouse", Precio = 15.50m, Categoria = "Perifericos", Stock = 200 }).GetAwaiter().GetResult();
        AddAsync(new Product { Nombre = "Monitor", Precio = 199.99m, Categoria = "Pantallas", Stock = 50 }).GetAwaiter().GetResult();
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        var list = _store.Values.OrderBy(p => p.Id).ToList();
        return Task.FromResult<IReadOnlyList<Product>>(list);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var value);
        return Task.FromResult(value);
    }

    public Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        var id = Interlocked.Increment(ref _idSeq);
        var copy = new Product
        {
            Id = id,
            Nombre = product.Nombre,
            Precio = product.Precio,
            Categoria = product.Categoria,
            Stock = product.Stock
        };
        _store[id] = copy;
        return Task.FromResult(copy);
    }

    public Task<bool> UpdateAsync(Product product, CancellationToken ct = default)
    {
        return Task.FromResult(_store.AddOrUpdate(product.Id, product, (_, __) => product) is not null);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(_store.TryRemove(id, out _));
    }

    public Task<IReadOnlyList<Product>> FilterAsync(string categoria, decimal precioMin, CancellationToken ct = default)
    {
        var q = _store.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(categoria))
            q = q.Where(p => string.Equals(p.Categoria, categoria, StringComparison.OrdinalIgnoreCase));
        q = q.Where(p => p.Precio >= precioMin);
        return Task.FromResult<IReadOnlyList<Product>>(q.OrderBy(p => p.Id).ToList());
    }
}
