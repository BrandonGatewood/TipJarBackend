using TipJar.Domain.Entities;

namespace TipJar.Application.ReadModels;

public sealed class UserReadModel
{
    public required decimal GrossTips { get; set; }
    public required IReadOnlyCollection<Tip> Tips { get; set; }
}