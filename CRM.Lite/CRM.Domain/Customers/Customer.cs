using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Customers;

public class Customer : AggregateRoot<int>
{
    public string Name { get; private set; } // 企业名称
    public string CreditCode { get; private set; } // 统一社会信用代码
    public string Industry { get; private set; } // 行业
    public Address Address { get; private set; } // 地址值对象

    // 私有集合 (充血模型核心)
    private readonly List<Contact> _contacts = new();
    public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();

    protected Customer() { }

    public Customer(string name, string creditCode, string industry, Address address)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessException("企业名称不能为空");

        Name = name;
        CreditCode = creditCode;
        Industry = industry;
        Address = address;
    }

    // 充血行为：添加联系人
    public void AddContact(string name, string phone, string title, bool isKeyDecisionMaker)
    {
        if (_contacts.Count >= 5) throw new BusinessException("单个客户联系人不能超过5个");
        if (_contacts.Any(c => c.Name == name)) throw new BusinessException("联系人姓名不能重复");

        _contacts.Add(new Contact(name, phone, title, isKeyDecisionMaker));
    }
}
