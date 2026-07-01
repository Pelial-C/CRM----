using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Web.Controllers.Api;

[Route("api/contracts")]
[Authorize(Roles = "Admin,Sales,EnterpriseUser")]
public class ContractsApiController : ApiControllerBase
{
    private readonly IContractAppService _contractAppService;

    public ContractsApiController(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ContractDto>>>> GetList([FromQuery] ContractListQueryDto query)
    {
        if (User.IsInRole("Sales")) query.OwnerUserId = GetCurrentUserId();
        return OkResponse(await _contractAppService.GetListAsync(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> Get(int id)
    {
        return OkResponse(await _contractAppService.GetAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> Create(CreateContractDto input)
    {
        if (User.IsInRole("Sales")) input.OwnerUserId = GetCurrentUserId();
        await _contractAppService.CreateAsync(input);
        return OkMessage("创建成功");
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, UpdateContractDto input)
    {
        input.Id = id;
        if (User.IsInRole("Sales")) input.OwnerUserId = GetCurrentUserId();
        await _contractAppService.UpdateAsync(input);
        return OkMessage("更新成功");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _contractAppService.DeleteAsync(id);
        return OkMessage("删除成功");
    }

    [HttpPost("{id:int}/start")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> Start(int id)
    {
        await _contractAppService.StartAsync(id);
        return OkMessage("合同已启动");
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Cancel(int id, [FromBody] ContractReasonDto? input)
    {
        await _contractAppService.CancelAsync(id, input?.Reason);
        return OkMessage("合同已作废");
    }

    [HttpPost("{id:int}/terminate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Terminate(int id, [FromBody] ContractReasonDto? input)
    {
        await _contractAppService.TerminateAsync(id, input?.Reason);
        return OkMessage("合同已终止");
    }

    [HttpPost("{id:int}/refresh-overdue")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> RefreshOverdue(int id)
    {
        await _contractAppService.RefreshOverduePaymentPlansAsync(id);
        return OkMessage("逾期状态已刷新");
    }

    [HttpGet("customers/{customerId:int}/contacts")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<List<ContactSelectDto>>>> GetContactsByCustomerId(int customerId)
    {
        return OkResponse(await _contractAppService.GetContactsByCustomerIdAsync(customerId));
    }

    [HttpPost("{id:int}/payment-plans/generate")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> GeneratePaymentPlans(int id)
    {
        await _contractAppService.GeneratePaymentPlansAsync(id);
        return OkMessage("回款计划已生成");
    }

    [HttpPost("{id:int}/payment-plans")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> AddPaymentPlan(int id, AddPaymentPlanDto input)
    {
        input.ContractId = id;
        await _contractAppService.AddPaymentPlanAsync(input);
        return OkMessage("回款计划已新增");
    }

    [HttpPost("{id:int}/payment-plans/{planId:int}/record-payment")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> RecordPayment(int id, int planId, RecordPaymentDto input)
    {
        input.ContractId = id;
        input.PlanId = planId;
        await _contractAppService.RecordPaymentAsync(input);
        return OkMessage("回款已登记");
    }

    private int? GetCurrentUserId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
    }
}

public class ContractReasonDto
{
    public string? Reason { get; set; }
}
