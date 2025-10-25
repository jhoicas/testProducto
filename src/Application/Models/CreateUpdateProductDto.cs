namespace ProductApi.Application.Models;

public record CreateUpdateProductDto(string Nombre, decimal Precio, string Categoria, int Stock);
