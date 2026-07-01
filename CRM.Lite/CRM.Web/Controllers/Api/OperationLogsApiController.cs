using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Application.Contracts.OperationLogs;
using CRM.Application.Contracts.OperationLogs.Dtos;
using CRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers.Api;

[Route("api/operation-logs")]
[Authorize]
public class OperationLogsApiController : ApiControllerBase
{
    private readonly IOperationLogAppService _operationLogAppService;

    public OperationLogsApiController(IOperationLogAppService operationLogAppService)
    {
        _operationLogAppService = operationLogAppService;
    }

    /// <summary>
    /// 分页查询操作日志
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<OperationLogDto>>>> GetPagedList(
        [FromQuery] OperationLogQueryDto query)
    {
        var result = await _operationLogAppService.GetPagedListAsync(query);
        return OkResponse(result);
    }
}
