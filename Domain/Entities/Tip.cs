namespace TipJar.Domain.Entities;

public class Tip(decimal amount, Guid userId) : BaseEntity
{
    private decimal _amount = amount;
    public decimal Amount => _amount;

    private readonly Guid _userId = userId;
    public Guid UserId => _userId;

    private DateTime _updatedAt;
    public DateTime UpdatedAt => _updatedAt;

    public void Edit(decimal amount, DateTime createdAt)
    {
        if (_amount != amount)
            _amount = amount;

        if(CreatedAt != createdAt)
            EditCreatedAt(createdAt);

        UpdateTime(DateTime.UtcNow);
    }

    private void UpdateTime(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
    }
}