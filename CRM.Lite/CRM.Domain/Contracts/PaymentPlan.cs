using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Contracts;

public class PaymentPlan : Entity<int>
{
    public int ContractId { get; private set; }
    public DateTime PlanDate { get; private set; }
    public decimal PlanAmount { get; private set; }
    public decimal ActualAmount { get; private set; }
    public DateTime? ActualDate { get; private set; }
    public PaymentPlanStatus Status { get; private set; }
    public string? Description { get; private set; }

    protected PaymentPlan() { }

    public PaymentPlan(DateTime planDate, decimal planAmount, string? description = null)
    {
        if (planAmount <= 0) throw new BusinessException("计划回款金额必须大于0");

        PlanDate = planDate;
        PlanAmount = planAmount;
        ActualAmount = 0;
        Status = PaymentPlanStatus.Pending;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void RecordActualPayment(decimal amount, DateTime date)
    {
        if (amount <= 0) throw new BusinessException("实际回款金额必须大于0");
        if (ActualAmount + amount > PlanAmount) throw new BusinessException("实际回款金额不能超过计划金额");

        ActualAmount += amount;
        ActualDate = date;
        Status = ActualAmount == PlanAmount ? PaymentPlanStatus.Paid : PaymentPlanStatus.PartialPaid;
    }

    public void MarkOverdue(DateTime today)
    {
        if (Status != PaymentPlanStatus.Paid && PlanDate.Date < today.Date)
        {
            Status = PaymentPlanStatus.Overdue;
        }
    }
}
