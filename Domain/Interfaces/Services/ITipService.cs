using TipJar.Application.Dtos.TipDto;

namespace TipJar.Domain.Interfaces.Services;

public interface ITipService
{
    Task<TipReceiptDto> AddTipAsync(decimal amount);
    Task<TipReceiptDto> EditTipAsync(Guid id, decimal amount, DateTime createdAt);
    Task DeleteTipAsync(Guid id);
    Task<MonthlyEarningsDto> GetThisMonthsEarningsAsync();
    Task<List<TipDto>> GetThisMonthsTipsAsync();
    Task<List<MonthlyInfoDto>> GetQuarterlyInfoAsync(int year, int quarter);
}