namespace CRM.Domain.OperationLogs;

/// <summary>
/// 操作日志实体 —— 记录领域事件产生的关键业务操作。
/// 继承 AggregateRoot&lt;int&gt; 以兼容泛型仓储，但不会引发自身的领域事件。
/// </summary>
public class OperationLog : AggregateRoot<int>
{
    /// <summary>事件类型名称，如 "CustomerCreatedEvent"</summary>
    public string EventType { get; private set; } = string.Empty;

    /// <summary>关联的聚合名称，如 "Customer"、"Contract"</summary>
    public string EntityName { get; private set; } = string.Empty;

    /// <summary>关联的聚合根 Id</summary>
    public int EntityId { get; private set; }

    /// <summary>人类可读的操作描述</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>操作人（当前版本为 null，后续可接入 ICurrentUser）</summary>
    public string? OperatorName { get; private set; }

    /// <summary>事件发生时间</summary>
    public DateTime OccurredAt { get; private set; }

    protected OperationLog() { }

    public OperationLog(
        string eventType,
        string entityName,
        int entityId,
        string description,
        string? operatorName,
        DateTime occurredAt)
    {
        EventType = eventType;
        EntityName = entityName;
        EntityId = entityId;
        Description = description;
        OperatorName = operatorName;
        OccurredAt = occurredAt;
    }
}
