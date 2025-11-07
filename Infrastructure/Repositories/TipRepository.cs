using Microsoft.EntityFrameworkCore;
using TipJar.Application.Dtos.TipDto;
using TipJar.Domain.Entities;
using TipJar.Domain.Interfaces.Repositories;
using TipJar.Infrastructure.Data;

namespace TipJar.Infrastructure.Repositories;

public class TipRepository(AppDbContext context) : ITipRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddAsync(Tip tip)
    {
        await _context.Tips.AddAsync(tip);
    }

    public async Task<decimal> GetThisMonthsEarningsAsync(Guid id, DateTime startOfMonth, DateTime startOfNextMonth)
    {
        return await _context.Tips
            .Where(t => t.UserId == id
                && t.CreatedAt >= startOfMonth
                && t.CreatedAt < startOfNextMonth)
            .SumAsync(t => t.Amount);
    }

    public async Task<List<TipDto>> GetThisMonthsTipsAsync(Guid id, DateTime startOfMonth, DateTime startOfNextMonth)
    {
        return await _context.Tips
            .Where(t => t.UserId == id
                && t.CreatedAt >= startOfMonth
                && t.CreatedAt < startOfNextMonth)
            .Select(t => new TipDto
            {
                Id = t.Id,
                Amount = t.Amount,
                CreatedAt = t.CreatedAt
            })
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<MonthlyInfoDto>> GetQuarterlyInfoAsync(Guid id, DateTime startOfQuarter, DateTime startOfNextQuarter)
    {
        return await _context.Tips
            .Where(
                t => t.UserId == id
                && t.CreatedAt >= startOfQuarter
                && t.CreatedAt < startOfNextQuarter
            )
            .GroupBy(t => t.CreatedAt.Month)
            .Select(g => new MonthlyInfoDto
            {
                Month = g.Key,
                Amount = g.Sum(t => t.Amount)
            })
            .OrderBy(x => x.Month)
            .ToListAsync();
    }
}