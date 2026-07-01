using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Application.Contracts.OperationLogs;
using CRM.Application.Contracts.OperationLogs.Dtos;
using CRM.Domain.OperationLogs;
using CRM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.OperationLogs;

public class OperationLogAppService : IOperationLogAppService
{
    private readonly IRepository<OperationLog, int> _logRepo;

    public OperationLogAppService(IRepository<OperationLog, int> logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task<PagedResultDto<OperationLogDto>> GetPagedListAsync(OperationLogQueryDto query)
    {
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 100);

        var q = _logRepo.Query();

        if (!string.IsNullOrWhiteSpace(query.EntityName))
        {
            var entityName = query.EntityName.Trim();
            q = q.Where(l => l.EntityName == entityName);
        }

        if (query.EntityId.HasValue && query.EntityId.Value > 0)
        {
            q = q.Where(l => l.EntityId == query.EntityId.Value);
        }

        if (query.StartDate.HasValue)
        {
            q = q.Where(l => l.OccurredAt >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            q = q.Where(l => l.OccurredAt <= query.EndDate.Value);
        }

        var totalCount = await q.CountAsync();

        var items = await q
            .OrderByDescending(l => l.OccurredAt)
            .ThenByDescending(l => l.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new OperationLogDto
            {
                Id = l.Id,
                EventType = l.EventType,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                Description = l.Description,
                OperatorName = l.OperatorName,
                OccurredAt = l.OccurredAt
            })
            .ToListAsync();

        return new PagedResultDto<OperationLogDto>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items
        };
    }
}
