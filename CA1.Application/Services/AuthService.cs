using System.Security.Cryptography;
using System.Text;
using CA1.Application.DTOs;
using CA1.Application.Interfaces;
using CA1.Domain.Entities;

namespace CA1.Application.Services;

/// <summary>
/// Servicio de Autenticación.
/// QUÉ: Maneja la lógica de registro y login.
/// CÓMO: 
/// 1. Verifica existencia de usuarios.
/// 2. Hasheda contraseñas (SHA256 simple para demo).
/// 3. Genera tokens JWT si las credenciales son válidas.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// POR QUÉ: Permite crear cuentas nuevas con roles específicos.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. Validar si ya existe
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            throw new Exception("User already exists.");
        }

        // 2. Crear entidad con password hasheada
        var user = new User
        {
            Username = request.Username,
            PasswordHash = HashPassword(request.Password),
            Role = request.Role
        };

        // 3. Persistir (Dapper)
        await _userRepository.AddAsync(user);

        // 4. Generar Token inmediato
        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Username);
    }

    /// <summary>
    /// Valida credenciales y emite token.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        // Comparación de Hash
        if (user == null || user.PasswordHash != HashPassword(request.Password))
        {
            throw new Exception("Invalid credentials.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Username);
    }

    /// <summary>
    /// Genera SHA256 del password.
    /// NOTA: En producción usar algoritmos más lentos como BCrypt o Argon2 con Salt.
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
