namespace TipJar.Application.ReadModels;

public sealed class MonthlyTipsReadModel
{
    public required decimal Total { get; set;}
    public required int Month { get; set;}
}