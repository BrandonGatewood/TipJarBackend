using System.Collections.Specialized;
using System.ComponentModel;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using TipJar.Application.Dtos.TipDto;
using TipJar.Application.Dtos.UserDto;
using TipJar.Application.Exceptions;
using TipJar.Application.ReadModels;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Security;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Application.Services;

public class UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IPasswordHasher passwordHasher, IDistributedCache distributedCache) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IDistributedCache _distributedCache = distributedCache;

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

        string cacheKey = $"user_info_{ userId }";

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);

        if(!string.IsNullOrEmpty(cachedData))
            return System.Text.Json.JsonSerializer.Deserialize<UserInfoDto>(cachedData) ?? throw new InvalidOperationException("Failed to deserialize cached user info.");

        UserReadModel user = await _userRepository.GetUserInfoByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        List<TipReadModel> recentTips = GetRecentTips([.. user.Tips]);
        decimal recentTipsTotal = recentTips.Sum(t => t.Amount);

        UserInfoDto userInfo = new()
        {
            GrossTips = user.Tips.Sum(t => t.Amount),
            Tips = GetUserTips([.. user.Tips]),
            YearlyEarnings = GetYearlyEarnings([.. user.Tips]),
            CurrentMonth = DateTime.UtcNow.ToString("MMMM"),
            RecentTipsTotal = recentTipsTotal,
            RecentTips = recentTips 
        };

        DistributedCacheEntryOptions cacheOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _distributedCache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize<UserInfoDto>(userInfo), cacheOptions);

        return userInfo;
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

    public async Task RemoveUserAsync()
    {
        Guid userId = GetUserId();

        User user = await _userRepository.GetByIdNoTipsAsync(userId) ?? throw new NotFoundException("User not found.");

        _userRepository.Remove(user);

        await _userRepository.SaveChangesAsync();
    }

    private static List<TipReadModel> GetUserTips(List<Tip> tips)
    {
        return [.. tips.Select(t => new TipReadModel
        {
            Id = t.Id,
            Amount = t.Amount,
            CreatedAt = t.CreatedAt.ToString("MM/dd/yyyy")
        })];
    }

    private static List<MonthlyTipsReadModel> GetYearlyEarnings(List<Tip> tips)
    {
        int year = DateTime.UtcNow.Year;

        var groupedByMonth = tips
            .Where(t => t.CreatedAt.Year == year)
            .GroupBy(t => t.CreatedAt.Month)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Amount)
            );

        return [.. Enumerable.Range(1, 12)
            .Select(month => new MonthlyTipsReadModel 
            {
                Total = groupedByMonth.TryGetValue(month, out var total)
                    ? total
                    : 0m,
                Month = month,
            })]; 
    }

    private static List<TipReadModel> GetRecentTips(List<Tip> tips)
    {
        DateTime now = DateTime.UtcNow;
        DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime startOfNextMonth = startOfMonth.AddMonths(1);

        return [.. tips.Where(t => t.CreatedAt >= startOfMonth
                && t.CreatedAt < startOfNextMonth)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TipReadModel
            {
                Id = t.Id,
                Amount = t.Amount,
                CreatedAt = t.CreatedAt.ToString("MM/dd/yyyy")
            })];
    }
}