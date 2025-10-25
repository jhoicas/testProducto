namespace ProductApi.Application.Models;

public record ProductDto(int Id, string Nombre, decimal Precio, string Categoria, int Stock);
