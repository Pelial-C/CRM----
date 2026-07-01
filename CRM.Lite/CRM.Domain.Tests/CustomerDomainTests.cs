using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Tests;

public class CustomerDomainTests
{
    [Fact]
    public void Customer_name_cannot_be_empty()
    {
        Assert.Throws<BusinessException>(() => new CRM.Domain.Customers.Customer("", null, null, Address.Empty));
    }

    [Fact]
    public void Deleted_customer_cannot_add_contact()
    {
        var customer = DomainTestFactory.CreateCustomer();

        customer.MarkAsDeleted();

        Assert.Throws<BusinessException>(() => customer.AddContact("张三", "13800000000", "经理"));
    }

    [Fact]
    public void Contact_name_and_phone_cannot_duplicate_in_same_customer()
    {
        var customer = DomainTestFactory.CreateCustomer();

        customer.AddContact("张三", "13800000000", "经理");

        Assert.Throws<BusinessException>(() => customer.AddContact("张三", "13800000000", "负责人"));
    }
}
