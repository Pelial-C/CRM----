namespace CRM.Domain;

public abstract class Entity<TKey>
{
    public TKey Id { get; protected set; }
}

// AggregateRoot.cs
public abstract class AggregateRoot<TKey> : Entity<TKey>
{
    public DateTime CreationTime { get; protected set; } = DateTime.Now;
}
