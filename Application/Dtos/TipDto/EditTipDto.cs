namespace TipJar.Application.Dtos.TipDto;

public class EditTipDto
{
    public required decimal Amount { get; set; }
    public required DateTime CreatedAt{ get; set; }
}