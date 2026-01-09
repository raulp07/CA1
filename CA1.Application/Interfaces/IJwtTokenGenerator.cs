using CA1.Domain.Entities;

namespace CA1.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
