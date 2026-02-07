using TipJar.Application.Dtos.TipDto;
using TipJar.Application.ReadModels;
using TipJar.Domain.Entities;

namespace TipJar.Domain.Interfaces.Repositories;

public interface ITipRepository
{
    Task AddAsync(Tip tip);
}