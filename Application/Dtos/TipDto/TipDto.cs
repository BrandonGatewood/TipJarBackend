namespace TipJar.Application.Dtos.TipDto;

public class TipDto 
{
    public required Guid Id { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime CreatedAt { get; set; } 
}