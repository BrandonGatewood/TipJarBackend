using TipJar.Application.Dtos.TipDto;
using TipJar.Domain.Entities;

namespace TipJar.Application.Mappings;

public static class TipMappings
{
    public static TipReceiptDto ToReceipt(this Tip tip)
    {
        return new TipReceiptDto
        {
            Amount = tip.Amount,
            CreatedAt = tip.CreatedAt
        };
    }
}