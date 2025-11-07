using TipJar.Application.Dtos.AuthDto;

namespace TipJar.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<TokenDto> LoginAsync(string username, string password);
    Task<TokenDto> RegisterAsync(string username, string password);
}