namespace CRM.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
