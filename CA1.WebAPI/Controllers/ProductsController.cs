using CA1.Application.DTOs;
using CA1.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA1.WebAPI.Controllers;

/// <summary>
/// Controlador de Productos.
/// QUÉ: Expone endpoints para gestión de productos.
/// CÓMO: Protegido por atributo [Authorize], requiere Bearer Token válido.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto productDto)
    {
        await _productService.CreateAsync(productDto);
        return Ok();
    }

    /// <summary>
    /// Actualizar Producto (PUT).
    /// QUÉ: Actualiza un recurso completo.
    /// CÓMO: Valida IDs y delega al servicio.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto productDto)
    {
        // Validación de Integridad: URL ID vs Body ID
        if (id != productDto.Id && productDto.Id != 0)
        {
            return BadRequest("ID mismatch between URL and Body.");
        }

        try
        {
            await _productService.UpdateAsync(id, productDto);
            // 204 No Content es el estándar para Updates exitosos que no devuelven datos
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Product {id} not found.");
        }
    }


    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     // Validación de Integridad: URL ID vs Body ID
    //     if (id != 0)
    //     {
    //         return BadRequest("ID mismatch between URL and Body.");
    //     }

    //     try
    //     {
    //         await _productService.DeleteAsync(id);
    //         // 204 No Content es el estándar para Updates exitosos que no devuelven datos
    //         return NoContent();
    //     }
    //     catch (KeyNotFoundException)
    //     {
    //         return NotFound($"Product {id} not found.");
    //     }
    // }
}
