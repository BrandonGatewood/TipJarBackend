namespace TipJar.Application.ReadModels;

public sealed class TipReadModel
{
    public required Guid Id { get; set; }
    public required decimal Amount { get; set; }
    public required string CreatedAt { get; set; } 
}

