using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Domain.Contracts;

public class PaymentPlan : Entity<int>
{
    public DateTime PlanDate { get; private set; }
    public decimal PlanAmount { get; private set; }
    public decimal ActualAmount { get; private set; }
    public DateTime? ActualDate { get; private set; }
    public string Description { get; private set; }

    protected PaymentPlan() { }

    public PaymentPlan(DateTime planDate, decimal planAmount, string description)
    {
        PlanDate = planDate;
        PlanAmount = planAmount;
        Description = description;
        ActualAmount = 0;
    }

    // 充血行为：记录实际回款
    public void RecordActualPayment(decimal amount, DateTime date)
    {
        if (amount <= 0) throw new Exception("回款金额必须大于0");
        ActualAmount += amount;
        ActualDate = date;
    }
}
