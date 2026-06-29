using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Customers;

public class Contact : Entity<int>
{
    public int CustomerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Title { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsKeyDecisionMaker { get; private set; }

    protected Contact() { }

    public Contact(int customerId, string name, string? phone, string? title, string? email = null, bool isKeyDecisionMaker = false)
        : this(name, phone, title, email, isKeyDecisionMaker)
    {
        if (customerId <= 0) throw new BusinessException("CustomerId必须有效");
        CustomerId = customerId;
    }

    internal Contact(string name, string? phone, string? title, string? email = null, bool isKeyDecisionMaker = false)
    {
        Update(name, phone, title, email, isKeyDecisionMaker);
    }

    public void Update(string name, string? phone, string? title, string? email = null, bool isKeyDecisionMaker = false)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessException("联系人姓名不能为空");

        Name = name.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        IsKeyDecisionMaker = isKeyDecisionMaker;
    }
}
