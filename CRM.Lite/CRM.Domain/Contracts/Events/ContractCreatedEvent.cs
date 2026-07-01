namespace CRM.Domain.Contracts.Events;

public sealed class ContractCreatedEvent : IDomainEvent
{
    public ContractCreatedEvent(int contractId, string contractNo, int customerId, decimal totalAmount, DateTime occurredOn)
    {
        ContractId = contractId;
        ContractNo = contractNo;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredOn = occurredOn;
    }

    public int ContractId { get; }
    public string ContractNo { get; }
    public int CustomerId { get; }
    public decimal TotalAmount { get; }
    public DateTime OccurredOn { get; }
}
