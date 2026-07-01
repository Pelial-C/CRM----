using CRM.Domain;
using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Shared.Enums;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Tests;

internal static class DomainTestFactory
{
    public static Customer CreateCustomer()
    {
        return new Customer("测试客户", "913100000000000000", "信息技术", Address.Empty);
    }

    public static Contract CreateContract(
        string contractNo = "HT-TEST",
        decimal totalAmount = 100,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var contract = new Contract(
            contractNo,
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

    public static void SetId(Entity<int> entity, int id)
    {
        typeof(Entity<int>).GetProperty(nameof(Entity<int>.Id))!.SetValue(entity, id);
    }
}
