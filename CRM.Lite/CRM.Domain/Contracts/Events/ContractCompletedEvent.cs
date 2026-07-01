namespace CRM.Domain.Contracts.Events;

public sealed class ContractCompletedEvent : IDomainEvent
{
    public ContractCompletedEvent(int contractId, string contractNo, DateTime occurredOn)
    {
        ContractId = contractId;
        ContractNo = contractNo;
        OccurredOn = occurredOn;
    }

    public int ContractId { get; }
    public string ContractNo { get; }
    public DateTime OccurredOn { get; }
}
