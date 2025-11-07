using TipJar.Application.Dtos.TipDto;
using TipJar.Domain.Entities;

namespace TipJar.Domain.Interfaces.Repositories;

public interface ITipRepository
{
    Task AddAsync(Tip tip);
    Task<decimal> GetThisMonthsEarningsAsync(Guid id, DateTime startOfMonth, DateTime startOfNextMonth);
    Task<List<TipDto>> GetThisMonthsTipsAsync(Guid id, DateTime startOfMonth, DateTime startOfNextMonth);
    Task<List<MonthlyInfoDto>> GetQuarterlyInfoAsync(Guid id, DateTime startOfQuarter, DateTime startOfNextQuarter);
}