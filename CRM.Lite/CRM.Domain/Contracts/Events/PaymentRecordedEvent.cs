namespace CRM.Domain.Contracts.Events;

public sealed class PaymentRecordedEvent : IDomainEvent
{
    public PaymentRecordedEvent(int contractId, int paymentPlanId, decimal actualAmount, DateTime actualDate, DateTime occurredOn)
    {
        ContractId = contractId;
        PaymentPlanId = paymentPlanId;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
        OccurredOn = occurredOn;
    }

    public int ContractId { get; }
    public int PaymentPlanId { get; }
    public decimal ActualAmount { get; }
    public DateTime ActualDate { get; }
    public DateTime OccurredOn { get; }
}
