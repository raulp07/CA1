namespace CA1.Application.DTOs;

/// <summary>
/// DTO para la solicitud de inicio de sesión.
/// POR QUÉ: Desacopla la estructura de la API de la entidad de dominio y valida entrada básica.
/// </summary>
public record LoginRequest(string Username, string Password);

/// <summary>
/// DTO para registro de nuevos usuarios. Incluye el Rol opcional.
/// </summary>
public record RegisterRequest(string Username, string Password, string Role = "User");

/// <summary>
/// Respuesta devuelta tras una autenticación exitosa.
/// QUÉ: Contiene el Token JWT y el usuario.
/// POR QUÉ: El cliente necesita el token para futuras peticiones autenticadas.
/// </summary>
public record AuthResponse(string Token, string Username);

/// <summary>
/// DTO para transferir datos de productos.
/// CÓMO: Se usa para recibir datos del cliente (POST) y para enviar datos al cliente (GET).
/// POR QUÉ: Evita exponer la entidad 'Product' completa directamente (por seguridad y versionado).
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
