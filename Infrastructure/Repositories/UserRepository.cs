using Microsoft.EntityFrameworkCore;
using TipJar.Application.Dtos.UserDto;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Infrastructure.Data;

namespace TipJar.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<UserLoginInfoDto?> GetForLoginAsync(string username)
    {
        return await _context.Users
            .Where(u => u.Username == username)
            .Select(u => new UserLoginInfoDto
            {
                Id = u.Id,
                Username = u.Username,
                PasswordHash = u.PasswordHash
            })
            .AsNoTracking()
            .SingleOrDefaultAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Tips)
            .SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByIdNoTipsAsync(Guid id)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserInfoDto?> GetUserInfoByIdAsync(Guid id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserInfoDto
            {
                Username = u.Username,
                GrossTips = u.GrossTips
            })
            .SingleOrDefaultAsync();
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}