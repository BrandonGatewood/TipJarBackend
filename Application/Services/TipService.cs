using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using TipJar.Application.Exceptions;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Services;

namespace TipJar.Application.Services;

/// <summary>
/// Provides operations for managing user tips, including creating and updating tips.
/// Handles validation, persistence, and cache invalidation for tip-related actions.
/// </summary>
public class TipService(IUserService userService, IUserRepository userRepository, ITipRepository tipRepository, IDistributedCache distributedCache) : ITipService
{
    private readonly IUserService _userService = userService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITipRepository _tipRepository = tipRepository;
    private readonly IDistributedCache _distributedCache = distributedCache;

    /// <summary>
    /// Creates a new tip for the currently authenticated user.
    /// </summary>
    /// <param name="amount">The tip amount to add.</param>
    /// <exception cref="InvalidTipAmountException">
    /// Thrown when the tip amount is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the current user cannot be found.
    /// </exception>
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

    /// <summary>
    /// Updates an existing tip for the currently authenticated user.
    /// </summary>
    /// <param name="id">The identifier of the tip to update.</param>
    /// <param name="amount">The new tip amount.</param>
    /// <param name="createdAt">
    /// The tip date in MM/dd/yyyy format.
    /// </param>
    /// <exception cref="InvalidTipAmountException">
    /// Thrown when the tip amount is less than or equal to zero.
    /// </exception>
    /// <exception cref="InvalidDateFormatException">
    /// Thrown when the date is not in MM/dd/yyyy format.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the current user cannot be found.
    /// </exception>
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
            string cacheKey = $"user_info_{ userId }";
            await _distributedCache.RemoveAsync(cacheKey);
        }
        catch(FormatException)
        {
            throw new InvalidDateFormatException("Date must be in MM/dd/yyyy format.");
        }
    }
}
