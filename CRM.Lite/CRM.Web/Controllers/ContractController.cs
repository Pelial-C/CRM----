using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers;

public class ContractController : Controller
{
    private readonly IContractAppService _contractAppService;

    public ContractController(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Create() => View();

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contract = await _contractAppService.GetAsync(id);
        var updateDto = new UpdateContractDto
        {
            Id = contract.Id,
            ContractNo = contract.ContractNo,
            ContractName = contract.ContractName,
            CabinetNo = contract.CabinetNo,
            SignDate = contract.SignDate,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            TotalAmount = contract.TotalAmount,
            Status = contract.Status,
            CustomerId = contract.CustomerId,
            ContactId = contract.ContactId,
            ServiceType = (CRM.Domain.Shared.Enums.ServiceType)contract.ServiceType,
            ContractType = (CRM.Domain.Shared.Enums.ContractType)contract.ContractType,
            PaymentFrequency = (CRM.Domain.Shared.Enums.PaymentFrequency)contract.PaymentFrequency,
            WarningDays = contract.WarningDays,
            RegionalCompany = contract.RegionalCompany,
            AffiliatedCompany = contract.AffiliatedCompany,
            Remark = contract.Remark,
            Items = contract.Items
        };
        return View(updateDto);
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        ViewBag.ContractId = id;
        return View();
    }

    /// <summary>
    /// 后端自证页面（仅供本地验收，非正式前端交付物）
    /// </summary>
    [HttpGet]
    public IActionResult SelfVerify() => View();

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetContractListDto input)
    {
        try
        {
            var data = await _contractAppService.GetListAsync(input);
            return Json(new { code = 200, msg = "success", data });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var data = await _contractAppService.GetAsync(id);
            return Json(new { code = 200, msg = "success", data });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetContactsByCustomerId(int customerId)
    {
        try
        {
            var data = await _contractAppService.GetContactsByCustomerIdAsync(customerId);
            return Json(new { code = 200, msg = "success", data });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractDto input)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
        }

        try
        {
            await _contractAppService.CreateAsync(input);
            return Json(new { code = 200, msg = "创建成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit([FromBody] UpdateContractDto input)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
        }

        try
        {
            await _contractAppService.UpdateAsync(input);
            return Json(new { code = 200, msg = "更新成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel([FromBody] CancelContractDto input)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
        }

        try
        {
            await _contractAppService.CancelAsync(input);
            return Json(new { code = 200, msg = "作废成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePaymentPlans([FromBody] GeneratePaymentPlansDto input)
    {
        try
        {
            await _contractAppService.GeneratePaymentPlansAsync(input.ContractId);
            return Json(new { code = 200, msg = "回款计划生成成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddPaymentPlan([FromBody] AddPaymentPlanDto input)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
        }

        try
        {
            await _contractAppService.AddPaymentPlanAsync(input);
            return Json(new { code = 200, msg = "回款计划添加成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto input)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
        }

        try
        {
            await _contractAppService.RecordPaymentAsync(input);
            return Json(new { code = 200, msg = "回款登记成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }
}
