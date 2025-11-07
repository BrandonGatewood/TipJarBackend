using System.Security.Claims;
using TipJar.Application.Dtos.UserDto;
using TipJar.Application.Exceptions;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Security;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Application.Services;

public class UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IPasswordHasher passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public Guid GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null.");
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("User Id claim not found.");
        if (!Guid.TryParse(userIdClaim.Value, out var userId)) throw new BadRequestException("Invalid User Id claim.");

        return userId;
    }

    public async Task<UserInfoDto> GetUserInfoAsync()
    {
        Guid userId = GetUserId();

        return await _userRepository.GetUserInfoByIdAsync(userId) ?? throw new NotFoundException("User not found.");
    }

    public async Task ChangePasswordAsync(string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new InvalidPasswordException("Password cannot be empty.");

        Guid userId = GetUserId();

        User user = await _userRepository.GetByIdNoTipsAsync(userId) ?? throw new NotFoundException("User not found.");

        if (_passwordHasher.Verify(currentPassword, user.PasswordHash))
            throw new InvalidPasswordException("Invalid password.");

        string newPasswordHash = _passwordHasher.Hash(newPassword);

        user.ChangePassword(newPasswordHash);

        await _userRepository.SaveChangesAsync();
    }

    public async Task ChangeUsernameAsync(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new InvalidUsernameException("Username cannot be empty.");

        bool usernameExists = await _userRepository.ExistsByUsernameAsync(newUsername);

        if (usernameExists)
            throw new InvalidUsernameException("username already exists.");

        Guid userId = GetUserId();

        User user = await _userRepository.GetByIdNoTipsAsync(userId) ?? throw new NotFoundException("User not found.");

        user.ChangeUsername(newUsername);

        await _userRepository.SaveChangesAsync();
    }

    public async Task RemoveUserAsync()
    {
        Guid userId = GetUserId();

        User user = await _userRepository.GetByIdNoTipsAsync(userId) ?? throw new NotFoundException("User not found.");

        _userRepository.Remove(user);

        await _userRepository.SaveChangesAsync();
    }
}