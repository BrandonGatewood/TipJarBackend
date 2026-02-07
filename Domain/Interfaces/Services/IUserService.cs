using TipJar.Application.Dtos.UserDto;

namespace TipJar.Domain.Interfaces.Services;

public interface IUserService
{
    Guid GetUserId();
    Task<UserInfoDto> GetUserInfoAsync();
    Task ChangePasswordAsync(string currentPassword, string newPassword);
    Task RemoveUserAsync();
}