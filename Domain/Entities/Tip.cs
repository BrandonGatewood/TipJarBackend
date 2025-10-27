namespace TipJar.Domain.Entities;

public class Tip(decimal amount, Guid userId) : BaseEntity
{
    private decimal _amount = amount;
    public decimal Amount => _amount;

    private readonly Guid _userId = userId;
    public Guid UserId => _userId;

    private readonly User? _user;
    public User? User => _user;

    private DateTime _updatedAt;
    public DateTime UpdatedAt => _updatedAt;

    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Tip amount must be positive.");
        }

        _amount = amount;

        UpdateTime(DateTime.UtcNow);
    }

    private void UpdateTime(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
    }
}