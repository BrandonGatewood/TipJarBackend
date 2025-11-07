namespace TipJar.Domain.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(Guid id);
}