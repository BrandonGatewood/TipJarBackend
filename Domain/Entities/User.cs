using TipJar.Application.Dtos.TipDto;
using TipJar.Application.Mappings;

namespace TipJar.Domain.Entities;

public class User : BaseEntity
{
    private string _username = string.Empty;
    public string Username => _username;

    private string _passwordHash = string.Empty;
    public string PasswordHash => _passwordHash;

    private decimal _grossTips;
    public decimal GrossTips => _grossTips;

    private readonly List<Tip> _tips = [];

    public IReadOnlyCollection<Tip> Tips => _tips.AsReadOnly();

    private User() { }
    
    public User(string username, string passwordHash)
    {
        _username = username;
        _passwordHash = passwordHash;
    }


    public void ChangeUsername(string newUsername)
    {
        _username = newUsername;
    }

    public void ChangePassword(string newPasswordHash)
    {
        _passwordHash = newPasswordHash;
    }

    public Tip AddTip(decimal amount)
    {
        Tip tip = new(amount, Id);
        UpdateGrossTips(amount);
        _tips.Add(tip);

        return tip;
    }

    public TipReceiptDto? EditTip(Guid tipId, decimal amount, DateTime createdAt)
    {
        foreach (Tip tip in _tips)
        {
            if (tip.Id == tipId)
            {
                UpdateGrossTips(-tip.Amount);
                tip.Edit(amount, createdAt);
                UpdateGrossTips(amount);

                return tip.ToReceipt();
            }
        }

        return null;
    }

    public bool DeleteTip(Guid tipId)
    {
        foreach (Tip tip in _tips)
        {
            if (tip.Id == tipId)
            {
                UpdateGrossTips(-tip.Amount);
                _tips.Remove(tip);

                return true;
            }
        }

        return false;
    }

    private void UpdateGrossTips(decimal amount)
    {
        _grossTips += amount;
    }
}