using System.Data;
using CA1.Application.Interfaces;
using CA1.Domain.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace CA1.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Usuarios usando Dapper (Micro-ORM).
/// QUÉ: Implementa acceso a datos de alto rendimiento para usuarios.
/// CÓMO: Ejecuta consultas SQL crudas directamente contra SQLite.
/// POR QUÉ: Dapper es más rápido que EF Core para lecturas y permite control total del SQL, ideal para Auth.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Crea una conexión nueva a SQLite. Dapper extiende esta conexión.
    /// </summary>
    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        // CÓMO: SQL paramétrico para evitar inyección SQL (@Username)
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Users WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task AddAsync(User user)
    {
        using var connection = CreateConnection();
        // CÓMO: Insertar y devolver el ID generado en una sola operación
        var sql = @"
            INSERT INTO Users (Username, PasswordHash, Role) 
            VALUES (@Username, @PasswordHash, @Role);
            SELECT last_insert_rowid();";
            
        var id = await connection.ExecuteScalarAsync<int>(sql, user);
        user.Id = id;
    }
}
