namespace CA1.Domain.Entities;

/// <summary>
/// Representa un usuario en el sistema.
/// QUÉ: Es la entidad central para la autenticación y autorización.
/// CÓMO: Se mapea a la tabla 'Users' en la base de datos (vía Dapper o EF).
/// POR QUÉ: Necesario para identificar quién accede al sistema y qué rol tiene.
/// </summary>
public class User
{
    /// <summary>Identificador único del usuario.</summary>
    public int Id { get; set; }
    
    /// <summary>Nombre de usuario para el login.</summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Hash de la contraseña.
    /// POR QUÉ: Nunca guardamos contraseñas en texto plano por seguridad.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Rol del usuario (ej. 'Admin', 'User').
    /// POR QUÉ: Para gestionar permisos (Autorización basada en roles).
    /// </summary>
    public string Role { get; set; } = "User";
}
