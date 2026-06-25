namespace CRM.Domain.Contracts;

public enum PaymentPlanStatus
{
    Pending = 0,
    PartialPaid = 1,
    Paid = 2,
    Overdue = 3
}
