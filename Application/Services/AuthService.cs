using TipJar.Application.Dtos.AuthDto;
using TipJar.Application.Dtos.UserDto;
using TipJar.Application.Exceptions;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Security;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Application.Services;

public class AuthService(IUserRepository userRepository, IJwtService jwtService, IPasswordHasher passwordHasher) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<TokenDto> LoginAsync(string username, string password)
    {
        UserLoginInfoDto? user = await _userRepository.GetForLoginAsync(username);

        if (user == null || _passwordHasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");


        TokenDto token = new()
        {
            Token = _jwtService.GenerateToken(user.Id)
        };

        return token;
    }

    public async Task<TokenDto> RegisterAsync(string username, string password)
    {
        bool usernameExists = await _userRepository.ExistsByUsernameAsync(username);

        if (usernameExists)
            throw new ConflictException("username already exists.");

        string passwordHash = _passwordHasher.Hash(password);

        User newUser = new(username, passwordHash);

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        TokenDto token = new()
        {
            Token = _jwtService.GenerateToken(newUser.Id)
        };

        return token;
    }
}