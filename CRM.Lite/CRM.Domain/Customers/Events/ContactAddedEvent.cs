namespace CRM.Domain.Customers.Events;

public sealed class ContactAddedEvent : IDomainEvent
{
    public ContactAddedEvent(int customerId, string customerName, string contactName, string? phone, DateTime occurredOn)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        ContactName = contactName;
        Phone = phone;
        OccurredOn = occurredOn;
    }

    public int CustomerId { get; }
    public string CustomerName { get; }
    public string ContactName { get; }
    public string? Phone { get; }
    public DateTime OccurredOn { get; }
}
