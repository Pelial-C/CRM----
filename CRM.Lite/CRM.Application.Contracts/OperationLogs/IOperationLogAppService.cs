using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Application.Contracts.OperationLogs.Dtos;

namespace CRM.Application.Contracts.OperationLogs;

public interface IOperationLogAppService
{
    Task<PagedResultDto<OperationLogDto>> GetPagedListAsync(OperationLogQueryDto query);
}
