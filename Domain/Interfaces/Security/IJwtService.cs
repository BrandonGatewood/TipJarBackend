namespace TipJar.Domain.Interfaces.Security;

/// <summary>
/// Contract for generating JWT for authenticated users.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a signed JWT for the specified user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A JWT string representing the authenticated user..</returns>
    string GenerateToken(Guid id);
}