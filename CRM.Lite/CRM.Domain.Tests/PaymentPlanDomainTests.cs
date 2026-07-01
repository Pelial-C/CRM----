using CRM.Domain.Contracts;
using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Tests;

public class PaymentPlanDomainTests
{
    [Fact]
    public void Actual_payment_cannot_exceed_plan_amount()
    {
        var contract = DomainTestFactory.CreateContract(totalAmount: 100);
        var plan = contract.AddPaymentPlan(DateTime.Today, 50);
        DomainTestFactory.SetId(plan, 1);

        Assert.Throws<BusinessException>(() => contract.RecordPayment(plan.Id, 60, DateTime.Today));
    }

    [Fact]
    public void Contract_completes_when_all_payment_plans_are_paid()
    {
        var contract = DomainTestFactory.CreateContract(totalAmount: 100);
        var firstPlan = contract.AddPaymentPlan(DateTime.Today, 40);
        var secondPlan = contract.AddPaymentPlan(DateTime.Today.AddDays(10), 60);
        DomainTestFactory.SetId(firstPlan, 1);
        DomainTestFactory.SetId(secondPlan, 2);

        contract.StartExecution();
        contract.RecordPayment(firstPlan.Id, 40, DateTime.Today);
        Assert.Equal(ContractStatus.Executing, contract.Status);

        contract.RecordPayment(secondPlan.Id, 60, DateTime.Today);

        Assert.Equal(ContractStatus.Completed, contract.Status);
    }

    [Fact]
    public void Overdue_payment_plan_can_be_marked_overdue()
    {
        var plan = new PaymentPlan(DateTime.Today.AddDays(-1), 100);

        plan.MarkOverdue(DateTime.Today);

        Assert.Equal(PaymentPlanStatus.Overdue, plan.Status);
    }
}
