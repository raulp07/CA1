using CA1.Application.Interfaces;
using CA1.Domain.Entities;
using CA1.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CA1.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Productos usando Entity Framework Core.
/// QUÉ: Implementa el acceso a datos para la entidad Product.
/// CÓMO: Usa 'AppDbContext' para abstracción de la base de datos.
/// POR QUÉ: EF Core simplifica el CRUD estándar sin escribir SQL manual.
/// NUEVO: Usa un 'Primary Constructor' (C# 12) recibiendo 'context' directamente en la firma de la clase.
/// </summary>
public class ProductRepository(AppDbContext context) : IProductRepository
{
    /* 
     * --- CÓDIGO ANTERIOR (Sin Primary Constructor) ---
     * private readonly AppDbContext _context;
     *
     * public ProductRepository(AppDbContext context)
     * {
     *     _context = context;
     * }
     */

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
