namespace TipJar.Application.Dtos.UserDto;

public class UserLoginInfoDto 
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
}
