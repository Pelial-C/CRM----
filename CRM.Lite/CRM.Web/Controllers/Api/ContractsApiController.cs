using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers.Api;

[Route("api/contracts")]
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
        return OkResponse(await _contractAppService.GetListAsync(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ContractDto>>> Get(int id)
    {
        return OkResponse(await _contractAppService.GetAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create(CreateContractDto input)
    {
        await _contractAppService.CreateAsync(input);
        return OkMessage("创建成功");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, UpdateContractDto input)
    {
        input.Id = id;
        await _contractAppService.UpdateAsync(input);
        return OkMessage("更新成功");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _contractAppService.DeleteAsync(id);
        return OkMessage("删除成功");
    }

    [HttpPost("{id:int}/start")]
    public async Task<ActionResult<ApiResponse<object>>> Start(int id)
    {
        await _contractAppService.StartAsync(id);
        return OkMessage("合同已启动");
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ApiResponse<object>>> Cancel(int id, [FromBody] ContractReasonDto? input)
    {
        await _contractAppService.CancelAsync(id, input?.Reason);
        return OkMessage("合同已作废");
    }

    [HttpPost("{id:int}/terminate")]
    public async Task<ActionResult<ApiResponse<object>>> Terminate(int id, [FromBody] ContractReasonDto? input)
    {
        await _contractAppService.TerminateAsync(id, input?.Reason);
        return OkMessage("合同已终止");
    }

    [HttpPost("{id:int}/refresh-overdue")]
    public async Task<ActionResult<ApiResponse<object>>> RefreshOverdue(int id)
    {
        await _contractAppService.RefreshOverduePaymentPlansAsync(id);
        return OkMessage("逾期状态已刷新");
    }

    [HttpGet("customers/{customerId:int}/contacts")]
    public async Task<ActionResult<ApiResponse<List<ContactSelectDto>>>> GetContactsByCustomerId(int customerId)
    {
        return OkResponse(await _contractAppService.GetContactsByCustomerIdAsync(customerId));
    }

    [HttpPost("{id:int}/payment-plans/generate")]
    public async Task<ActionResult<ApiResponse<object>>> GeneratePaymentPlans(int id)
    {
        await _contractAppService.GeneratePaymentPlansAsync(id);
        return OkMessage("回款计划已生成");
    }

    [HttpPost("{id:int}/payment-plans")]
    public async Task<ActionResult<ApiResponse<object>>> AddPaymentPlan(int id, AddPaymentPlanDto input)
    {
        input.ContractId = id;
        await _contractAppService.AddPaymentPlanAsync(input);
        return OkMessage("回款计划已新增");
    }

    [HttpPost("{id:int}/payment-plans/{planId:int}/record-payment")]
    public async Task<ActionResult<ApiResponse<object>>> RecordPayment(int id, int planId, RecordPaymentDto input)
    {
        input.ContractId = id;
        input.PlanId = planId;
        await _contractAppService.RecordPaymentAsync(input);
        return OkMessage("回款已登记");
    }
}

public class ContractReasonDto
{
    public string? Reason { get; set; }
}
