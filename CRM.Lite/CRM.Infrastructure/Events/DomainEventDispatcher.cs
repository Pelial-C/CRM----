using CRM.Domain;
using CRM.Domain.Contracts.Events;
using CRM.Domain.Customers.Events;
using CRM.Domain.Events;
using CRM.Domain.OperationLogs;
using CRM.Infrastructure.Persistence;

namespace CRM.Infrastructure.Events;

/// <summary>
/// 领域事件分发器 —— 将领域事件翻译为操作日志并持久化到数据库。
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly CrmDbContext _dbContext;

    public DomainEventDispatcher(CrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var domainEvent in events)
        {
            var log = MapToOperationLog(domainEvent);
            if (log != null)
            {
                _dbContext.OperationLogs.Add(log);
            }
        }

        if (events.Count > 0)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    private static OperationLog? MapToOperationLog(IDomainEvent @event)
    {
        return @event switch
        {
            CustomerCreatedEvent e => new OperationLog(
                nameof(CustomerCreatedEvent),
                "Customer",
                e.CustomerId,
                $"创建客户：{e.CustomerName}",
                null,
                e.OccurredOn),

            ContactAddedEvent e => new OperationLog(
                nameof(ContactAddedEvent),
                "Customer",
                e.CustomerId,
                $"客户 {e.CustomerName} 新增联系人 {e.ContactName}",
                null,
                e.OccurredOn),

            ContractCreatedEvent e => new OperationLog(
                nameof(ContractCreatedEvent),
                "Contract",
                e.ContractId,
                $"创建合同：{e.ContractNo}，金额 {e.TotalAmount:N2}",
                null,
                e.OccurredOn),

            ContractCompletedEvent e => new OperationLog(
                nameof(ContractCompletedEvent),
                "Contract",
                e.ContractId,
                $"合同完成：{e.ContractNo}",
                null,
                e.OccurredOn),

            ContractCancelledEvent e => new OperationLog(
                nameof(ContractCancelledEvent),
                "Contract",
                e.ContractId,
                $"合同作废：{e.ContractNo}，原因：{e.CancelReason}",
                null,
                e.OccurredOn),

            PaymentRecordedEvent e => new OperationLog(
                nameof(PaymentRecordedEvent),
                "Contract",
                e.ContractId,
                $"登记回款：{e.ActualAmount:N2}，日期 {e.ActualDate:yyyy-MM-dd}",
                null,
                e.OccurredOn),

            _ => null
        };
    }
}
