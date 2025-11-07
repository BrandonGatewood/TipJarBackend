using TipJar.Application.Dtos.UserDto;
using TipJar.Domain.Entities;

namespace TipJar.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserLoginInfoDto?> GetForLoginAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByIdNoTipsAsync(Guid id);
    Task<UserInfoDto?> GetUserInfoByIdAsync(Guid id);
    Task<bool> ExistsByUsernameAsync(string username);
    Task AddAsync(User user);
    void Remove(User user);
    Task SaveChangesAsync();
}