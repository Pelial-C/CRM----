namespace CRM.Domain.Customers.Events;

public sealed class CustomerCreatedEvent : IDomainEvent
{
    public CustomerCreatedEvent(int customerId, string customerName, DateTime occurredOn)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        OccurredOn = occurredOn;
    }

    public int CustomerId { get; }
    public string CustomerName { get; }
    public DateTime OccurredOn { get; }
}
