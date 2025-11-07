using TipJar.Application.Dtos.TipDto;
using TipJar.Application.Exceptions;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Domain.Interfaces.Services;
using TipJar.Application.Mappings;

namespace TipJar.Application.Services;

public class TipService(IUserService userService, IUserRepository userRepository, ITipRepository tipRepository) : ITipService
{
    private readonly IUserService _userService = userService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITipRepository _tipRepository = tipRepository;


    public async Task<TipReceiptDto> AddTipAsync(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidTipAmountException("Tip amount must be positive.");

        Guid userId = _userService.GetUserId();
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        Tip tip = user.AddTip(amount);

        await _tipRepository.AddAsync(tip);
        await _userRepository.SaveChangesAsync();

        return tip.ToReceipt();
    }

    public async Task<TipReceiptDto> EditTipAsync(Guid id, decimal amount, DateTime createdAt)
    {
        if (amount <= 0)
            throw new InvalidTipAmountException("Tip amount must be positive.");

        Guid userId = _userService.GetUserId();
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        TipReceiptDto tip = user.EditTip(id, amount, createdAt) ?? throw new NotFoundException("Tip not found.");

        await _userRepository.SaveChangesAsync();

        return tip;
    }

    public async Task DeleteTipAsync(Guid id)
    {
        Guid userId = _userService.GetUserId();
        User user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        if (!user.DeleteTip(id))
            throw new NotFoundException("tip not found");

        await _userRepository.SaveChangesAsync();
    }

    public async Task<MonthlyEarningsDto> GetThisMonthsEarningsAsync()
    {
        Guid userId = _userService.GetUserId();

        DateTime now = DateTime.UtcNow;
        DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime startOfNextMonth = startOfMonth.AddMonths(1);

        decimal amount = await _tipRepository.GetThisMonthsEarningsAsync(userId, startOfMonth, startOfNextMonth);

        return new MonthlyEarningsDto
        {
            Amount = amount
        };
    }

    public async Task<List<TipDto>> GetThisMonthsTipsAsync()
    {
        Guid userId = _userService.GetUserId();

        DateTime now = DateTime.UtcNow;
        DateTime startOfMonth = new(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime startOfNextMonth = startOfMonth.AddMonths(1);

        return await _tipRepository.GetThisMonthsTipsAsync(userId, startOfMonth, startOfNextMonth);
    }

    public async Task<List<MonthlyInfoDto>> GetQuarterlyInfoAsync(int year, int quarter)
    {
        Guid userId = _userService.GetUserId();

        int startMonth = (quarter - 1) * 3 + 1;
        DateTime startOfQuarter = new(year, startMonth, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime startOfNextQuarter = startOfQuarter.AddMonths(3);

        return await _tipRepository.GetQuarterlyInfoAsync(userId, startOfQuarter, startOfNextQuarter);
    }
}
