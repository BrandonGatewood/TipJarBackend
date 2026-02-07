using Microsoft.EntityFrameworkCore;
using TipJar.Application.Dtos.TipDto;
using TipJar.Application.ReadModels;
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
}