using CA1.Application.DTOs;
using CA1.Application.Interfaces;
using CA1.Domain.Entities;

namespace CA1.Application.Services;

/// <summary>
/// Servicio de Productos.
/// QUÉ: Contiene la lógica de negocio para gestionar productos.
/// CÓMO: Interactúa con IProductRepository (EF Core) y transforma Entidades <-> DTOs.
/// NUEVO: Usa un 'Primary Constructor' (C# 12) para recibir las dependencias.
/// </summary>
public class ProductService(IProductRepository productRepository, AutoMapper.IMapper mapper) : IProductService
{
    /*
     * --- CÓDIGO ANTERIOR ---
     * private readonly IProductRepository _productRepository;
     * private readonly AutoMapper.IMapper _mapper;
     *
     * public ProductService(IProductRepository productRepository, AutoMapper.IMapper mapper)
     * {
     *     _productRepository = productRepository;
     *     _mapper = mapper;
     * }
     */

    /// <summary>
    /// Obtiene todos los productos y los mapea a DTOs.
    /// POR QUÉ: La capa de presentación no debe conocer las Entidades de Dominio directamente.
    /// </summary>
    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await productRepository.GetAllAsync();
        // Usando AutoMapper para proyectar la colección
        return mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product == null) return null;

        return mapper.Map<ProductDto>(product);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// CÓMO: 
    /// 1. Recibe DTO.
    /// 2. Mapea a Entidad de Dominio 'Product'.
    /// 3. Llama al repositorio para persistir.
    /// </summary>
    public async Task CreateAsync(ProductDto productDto)
    {
        // Mapeo con AutoMapper
        var product = mapper.Map<Product>(productDto);

        /* Mapeo Manual (Comentado)
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price
        };
        */

        await productRepository.AddAsync(product);
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// QUÉ: Modifica los valores de un producto en la BD.
    /// CÓMO: 
    /// 1. Verifica existencia (GetById).
    /// 2. Mapea cambios del DTO a la Entidad.
    /// 3. Persiste cambios.
    /// </summary>
    public async Task UpdateAsync(int id, ProductDto productDto)
    {
        // 1. Verificación básica: ¿Existe el producto?
        var existingProduct = await productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            // Lanzamos excepción si no existe (el Controller manejará esto)
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        // 2. Mapeo inteligente con AutoMapper
        // Esto toma los valores de 'productDto' y los escribe sobre 'existingProduct'
        mapper.Map(productDto, existingProduct);

        // 3. Integridad de Datos
        // Nos aseguramos de que el ID no haya sido alterado por el DTO
        existingProduct.Id = id;

        // 4. Persistencia
        await productRepository.UpdateAsync(existingProduct);
    }

    public async Task DeleteAsync(int id)
    {
        await productRepository.DeleteAsync(id);
    }
}
