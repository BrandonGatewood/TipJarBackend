namespace TipJar.Domain.Entities;

public class User(string username, string passwordHash, decimal grossTips) : BaseEntity
{
    private string _username = username;
    public string Username => _username;

    private string _passwordHash = passwordHash;
    public string PasswordHash => _passwordHash;

    private decimal _grossTips = grossTips;
    public decimal GrossTips => _grossTips;

    private readonly List<Tip> _tips = [];

    public IReadOnlyCollection<Tip> Tips => _tips;



    public void UpdateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.");

        _username = username;
    }

    internal void UpdatePasswordHashInternal(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password cannot be empty.");

        _passwordHash = passwordHash;
    }

    public void AddTip(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Tip amount must be positive.");

        Tip tip = new(amount, Id);
        UpdateGrossTips(amount);
        _tips.Add(tip);
    }

    public void UpdateTip(Guid tipId, decimal amount)
    {
        foreach (Tip tip in _tips)
        {
            if (tip.Id == tipId)
            {
                UpdateGrossTips(-tip.Amount);
                tip.UpdateAmount(amount);
                UpdateGrossTips(amount);

                break;
            }
        }
    }

    public void RemoveTip(Guid tipId)
    {
        foreach (Tip tip in _tips)
        {
            if (tip.Id == tipId)
            {
                UpdateGrossTips(-tip.Amount);
                _tips.Remove(tip);

                break;
            }
        }
    }

    private void UpdateGrossTips(decimal amount)
    {
        _grossTips += amount;
    }
}