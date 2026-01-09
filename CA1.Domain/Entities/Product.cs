namespace CA1.Domain.Entities;

/// <summary>
/// Representa un producto en el catálogo.
/// QUÉ: Entidad de negocio principal para la gestión de inventario.
/// CÓMO: Se mapea a la tabla 'Products' usando Entity Framework Core.
/// POR QUÉ: Permite persistir y recuperar información de productos de forma estructurada.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
