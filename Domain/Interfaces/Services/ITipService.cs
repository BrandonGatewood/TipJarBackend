using TipJar.Application.Dtos.TipDto;

namespace TipJar.Domain.Interfaces.Services;

public interface ITipService
{
    Task AddTipAsync(decimal amount);
    Task EditTipAsync(Guid id, decimal amount, string createdAt);
}