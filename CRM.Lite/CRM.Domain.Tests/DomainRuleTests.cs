using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Tests;

public class DomainRuleTests
{
    [Fact]
    public void Customer_name_cannot_be_empty()
    {
        Assert.Throws<BusinessException>(() => new Customer("", null, null, Address.Empty));
    }

    [Fact]
    public void Deleted_customer_cannot_add_contact()
    {
        var customer = CreateCustomer();

        customer.MarkAsDeleted();

        Assert.Throws<BusinessException>(() => customer.AddContact("张三", "13800000000", "经理"));
    }

    [Fact]
    public void Customer_delete_is_logical_delete()
    {
        var customer = CreateCustomer();

        customer.MarkAsDeleted();

        Assert.True(customer.IsDeleted);
    }

    [Fact]
    public void Contract_amount_must_be_positive()
    {
        Assert.Throws<BusinessException>(() => CreateContract(totalAmount: 0));
    }

    [Fact]
    public void Contract_end_date_cannot_be_before_start_date()
    {
        Assert.Throws<BusinessException>(() =>
            CreateContract(startDate: new DateTime(2026, 7, 2), endDate: new DateTime(2026, 7, 1)));
    }

    [Fact]
    public void Contract_items_must_have_at_least_one_item()
    {
        var contract = CreateContract();

        Assert.Throws<BusinessException>(() => contract.ReplaceItems(Array.Empty<(string, int, decimal)>()));
    }

    [Fact]
    public void Payment_plan_total_cannot_exceed_contract_amount()
    {
        var contract = CreateContract(totalAmount: 100);
        contract.AddPaymentPlan(DateTime.Today, 80);

        Assert.Throws<BusinessException>(() => contract.AddPaymentPlan(DateTime.Today, 30));
    }

    [Fact]
    public void Actual_payment_cannot_exceed_plan_amount()
    {
        var contract = CreateContract(totalAmount: 100);
        var plan = contract.AddPaymentPlan(DateTime.Today, 50);

        Assert.Throws<BusinessException>(() => contract.RecordPayment(plan.Id, 60, DateTime.Today));
    }

    [Fact]
    public void Cancelled_contract_cannot_record_payment()
    {
        var contract = CreateContract(totalAmount: 100);
        var plan = contract.AddPaymentPlan(DateTime.Today, 50);

        contract.Cancel("作废");

        Assert.Throws<BusinessException>(() => contract.RecordPayment(plan.Id, 50, DateTime.Today));
    }

    [Fact]
    public void Contract_completes_when_all_payment_plans_are_paid()
    {
        var contract = CreateContract(totalAmount: 100);
        var plan = contract.AddPaymentPlan(DateTime.Today, 100);

        contract.StartExecution();
        contract.RecordPayment(plan.Id, 100, DateTime.Today);

        Assert.Equal(ContractStatus.Completed, contract.Status);
    }

    private static Customer CreateCustomer()
    {
        return new Customer("测试客户", "913100000000000000", "信息技术", Address.Empty);
    }

    private static Contract CreateContract(
        decimal totalAmount = 100,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var contract = new Contract(
            "HT-TEST",
            "测试合同",
            1,
            "测试客户",
            null,
            null,
            totalAmount,
            DateTime.Today,
            startDate ?? DateTime.Today,
            endDate ?? DateTime.Today.AddMonths(1),
            PaymentFrequency.Monthly);

        contract.ReplaceItems(new[] { ("服务费", 1, totalAmount) });
        return contract;
    }
}
