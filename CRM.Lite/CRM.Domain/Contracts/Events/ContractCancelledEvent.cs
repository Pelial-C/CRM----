namespace CRM.Domain.Contracts.Events;

public sealed class ContractCancelledEvent : IDomainEvent
{
    public ContractCancelledEvent(int contractId, string contractNo, string cancelReason, DateTime occurredOn)
    {
        ContractId = contractId;
        ContractNo = contractNo;
        CancelReason = cancelReason;
        OccurredOn = occurredOn;
    }

    public int ContractId { get; }
    public string ContractNo { get; }
    public string CancelReason { get; }
    public DateTime OccurredOn { get; }
}
