using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using TipJar.Application.Dtos.TipDto;
using TipJar.Application.Exceptions;
using TipJar.Application.ReadModels;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Application.Services;

public class TipService(IUserService userService, IUserRepository userRepository, ITipRepository tipRepository, IDistributedCache distributedCache) : ITipService
{
    private readonly IUserService _userService = userService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITipRepository _tipRepository = tipRepository;
    private readonly IDistributedCache _distributedCache = distributedCache;


    public async Task AddTipAsync(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidTipAmountException("Tip amount must be positive.");

        Guid userId = _userService.GetUserId();
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        Tip tip = user.AddTip(amount);

        await _tipRepository.AddAsync(tip);
        await _userRepository.SaveChangesAsync();

        string cacheKey = $"user_info_{ userId }";
        await _distributedCache.RemoveAsync(cacheKey);
    }

    public async Task EditTipAsync(Guid id, decimal amount, string createdAt)
    {
        if (amount <= 0)
            throw new InvalidTipAmountException("Tip amount must be positive.");

        Guid userId = _userService.GetUserId();
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        try
        {
            var utcDate = DateTime.ParseExact(
                createdAt,
                "MM/dd/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            );

            user.EditTip(id, amount, utcDate);

            await _userRepository.SaveChangesAsync();
        }
        catch(FormatException)
        {
            throw new InvalidDateFormatException("Date must be in MM/dd/yyyy format.");
        }
    }
}
