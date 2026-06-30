using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Customers;

public class Customer : AggregateRoot<int>
{
    public string Name { get; private set; } = string.Empty;
    public string? CreditCode { get; private set; }
    public string? Industry { get; private set; }
    public Address Address { get; private set; } = Address.Empty;
    public string? Remark { get; private set; }
    public bool IsDeleted { get; private set; }

    private readonly List<Contact> _contacts = new();
    public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();

    protected Customer() { }

    public Customer(string name, string? creditCode, string? industry, Address address, string? remark = null)
    {
        UpdateInfo(name, creditCode, industry, address, remark);
    }

    public Contact AddContact(string name, string? phone, string? title, string? email = null, bool isKeyDecisionMaker = false)
    {
        if (IsDeleted) throw new BusinessException("已删除客户不能新增联系人");
        if (_contacts.Any(c => c.Name == name)) throw new BusinessException("联系人姓名不能重复");

        var contact = new Contact(name, phone, title, email, isKeyDecisionMaker);
        _contacts.Add(contact);
        return contact;
    }

    public void RemoveContact(int contactId)
    {
        if (IsDeleted) throw new BusinessException("已删除客户不能维护联系人");

        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null) throw new BusinessException("联系人不存在");

        _contacts.Remove(contact);
    }

    public void UpdateInfo(string name, string? creditCode, string? industry, Address address, string? remark = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessException("客户名称不能为空");

        Name = name.Trim();
        CreditCode = string.IsNullOrWhiteSpace(creditCode) ? null : creditCode.Trim();
        Industry = string.IsNullOrWhiteSpace(industry) ? null : industry.Trim();
        Address = address ?? Address.Empty;
        Remark = string.IsNullOrWhiteSpace(remark) ? null : remark.Trim();
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
