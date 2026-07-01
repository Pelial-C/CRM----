namespace CRM.Domain.Events;

/// <summary>
/// 领域事件分发器接口。
/// Infrastructure 层实现此接口，负责将领域事件翻译为操作日志并持久化。
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken ct = default);
}
