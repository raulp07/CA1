using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CA1.Application.Interfaces;
using CA1.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CA1.Infrastructure.Authentication;

/// <summary>
/// Generador de Tokens JWT.
/// CÓMO: 
/// 1. Define Claims super seguros (ID, Username, Rol).
/// 2. Firma el token con una clave secreta (HmacSha256).
/// 3. Asigna tiempo de expiración.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var secret = _configuration["JwtSettings:Secret"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims: Datos incrustados en el token
        // POR QUÉ: Para que el servidor sepa quién es el usuario en futuras peticiones sin ir a BD.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
