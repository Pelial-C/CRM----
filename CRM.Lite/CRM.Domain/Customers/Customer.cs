using CRM.Domain.Shared.Exceptions;
using CRM.Domain.Customers.Events;
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
    public int? OwnerUserId { get; private set; }

    private readonly List<Contact> _contacts = new();
    public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();

    protected Customer() { }

    public Customer(string name, string? creditCode, string? industry, Address address, string? remark = null)
    {
        UpdateInfo(name, creditCode, industry, address, remark);
        AddDomainEvent(new CustomerCreatedEvent(Id, Name, DateTime.Now));
    }

    public Contact AddContact(string name, string? phone, string? title, string? email = null, bool isKeyDecisionMaker = false)
    {
        if (IsDeleted) throw new BusinessException("已删除客户不能新增联系人");
        var normalizedName = name.Trim();
        var normalizedPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        if (_contacts.Any(c => c.Name == normalizedName && c.Phone == normalizedPhone))
        {
            throw new BusinessException("同一客户下联系人姓名和手机号不能重复");
        }

        var contact = new Contact(name, phone, title, email, isKeyDecisionMaker);
        _contacts.Add(contact);
        AddDomainEvent(new ContactAddedEvent(Id, Name, contact.Name, contact.Phone, DateTime.Now));
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

    public void SetOwner(int? ownerUserId)
    {
        if (ownerUserId.HasValue && ownerUserId.Value <= 0) throw new BusinessException("负责人用户ID无效");

        OwnerUserId = ownerUserId;
    }
}
