using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Tests;

public class ContractDomainTests
{
    [Fact]
    public void Contract_no_cannot_be_empty()
    {
        Assert.Throws<BusinessException>(() => DomainTestFactory.CreateContract(contractNo: ""));
    }

    [Fact]
    public void Contract_amount_must_be_positive()
    {
        Assert.Throws<BusinessException>(() => DomainTestFactory.CreateContract(totalAmount: 0));
    }

    [Fact]
    public void Contract_end_date_cannot_be_before_start_date()
    {
        Assert.Throws<BusinessException>(() =>
            DomainTestFactory.CreateContract(startDate: new DateTime(2026, 7, 2), endDate: new DateTime(2026, 7, 1)));
    }

    [Fact]
    public void Contract_items_cannot_be_empty()
    {
        var contract = CreateContractWithoutItems(totalAmount: 100);

        Assert.Throws<BusinessException>(() => contract.ReplaceItems(Array.Empty<(string, int, decimal)>()));
    }

    [Fact]
    public void Contract_item_total_must_equal_contract_total_amount()
    {
        var contract = CreateContractWithoutItems(totalAmount: 100);

        Assert.Throws<BusinessException>(() => contract.ReplaceItems(new[] { ("服务费", 1, 90m) }));
    }

    [Fact]
    public void Contract_item_unit_price_must_be_greater_than_zero()
    {
        var contract = CreateContractWithoutItems(totalAmount: 100);

        Assert.Throws<BusinessException>(() => contract.ReplaceItems(new[] { ("服务费", 1, 0m) }));
    }

    [Fact]
    public void Contract_cancel_reason_cannot_be_empty()
    {
        var contract = DomainTestFactory.CreateContract();

        Assert.Throws<BusinessException>(() => contract.Cancel(""));
    }

    [Fact]
    public void Cancelled_contract_cannot_record_payment()
    {
        var contract = DomainTestFactory.CreateContract(totalAmount: 100);
        var plan = contract.AddPaymentPlan(DateTime.Today, 100);
        DomainTestFactory.SetId(plan, 1);

        contract.Cancel("客户终止采购");

        Assert.Throws<BusinessException>(() => contract.RecordPayment(plan.Id, 100, DateTime.Today));
    }

    private static Contract CreateContractWithoutItems(decimal totalAmount)
    {
        return new Contract(
            "HT-ITEM",
            "测试合同",
            1,
            "测试客户",
            null,
            null,
            totalAmount,
            DateTime.Today,
            DateTime.Today,
            DateTime.Today.AddMonths(1),
            PaymentFrequency.Monthly);
    }
}
