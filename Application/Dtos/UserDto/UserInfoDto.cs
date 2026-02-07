using TipJar.Application.Dtos.TipDto;
using TipJar.Application.ReadModels;
using TipJar.Domain.Entities;

namespace TipJar.Application.Dtos.UserDto;

public class UserInfoDto
{
    public required decimal GrossTips { get; set; }
    public required IReadOnlyCollection<TipReadModel> Tips { get; set; }
    public required IReadOnlyCollection<MonthlyTipsReadModel> YearlyEarnings { get; set; }
    public required string CurrentMonth { get; set; }
    public required decimal RecentTipsTotal { get; set;}
    public required IReadOnlyCollection<TipReadModel> RecentTips { get; set;}
}