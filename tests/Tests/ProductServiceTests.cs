using ProductApi.Application.Abstractions;
using ProductApi.Application.Models;
using ProductApi.Application.Services;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using Xunit;

namespace ProductApi.Tests;

public class ProductServiceTests
{
    private readonly IProductRepository _repo;
    private readonly IProductService _service;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var ctx = new AppDbContext(options);
        // seed
        if (!ctx.Productos.Any())
        {
            ctx.Productos.AddRange(new[]
            {
                new ProductApi.Domain.Entities.Product{ Nombre = "Seed1", Precio = 10m, Categoria = "Cat1", Stock = 5 },
                new ProductApi.Domain.Entities.Product{ Nombre = "Seed2", Precio = 20m, Categoria = "Cat2", Stock = 8 }
            });
            ctx.SaveChanges();
        }
        _repo = new EfProductRepository(ctx);
        _service = new ProductService(_repo);
    }

    [Fact]
    public async Task Create_Should_Add_Product()
    {
        var dto = new CreateUpdateProductDto("Laptop", 1200m, "Computo", 10);
        var created = await _service.CreateAsync(dto);
        Assert.True(created.Id > 0);
        var fetched = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Laptop", fetched!.Nombre);
    }

    [Fact]
    public async Task Update_Should_Return_False_When_Not_Found()
    {
        var ok = await _service.UpdateAsync(9999, new CreateUpdateProductDto("X", 1m, "Cat", 1));
        Assert.False(ok);
    }

    [Fact]
    public async Task Filter_Should_Return_By_Category_And_MinPrice()
    {
        var all = await _service.GetAllAsync();
        var any = all.First();
        var result = await _service.FilterAsync(any.Categoria, 0.01m);
        Assert.True(result.All(p => p.Categoria.Equals(any.Categoria, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public async Task Create_Should_Validate_Input()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(new(" ", 10m, "Cat", 1)));
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(new("Ok", 0m, "Cat", 1)));
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(new("Ok", 10m, " ", 1)));
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(new("Ok", 10m, "Cat", -1)));
    }
}
