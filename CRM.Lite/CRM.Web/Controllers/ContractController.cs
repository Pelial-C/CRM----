using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
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
    public async Task<IActionResult> Index(string? keyword, int? status, int pageIndex = 1, int pageSize = 10)
    {
        var query = new GetContractListDto
        {
            Keyword = keyword,
            Status = status,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _contractAppService.GetListAsync(query);
        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var contract = await _contractAppService.GetAsync(id);
        return View(contract);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateContractDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateContractDto input)
    {
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _contractAppService.CreateAsync(input);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(input);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contract = await _contractAppService.GetAsync(id);

        var dto = new UpdateContractDto
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
            WarningDays = contract.WarningDays,
            RegionalCompany = contract.RegionalCompany,
            AffiliatedCompany = contract.AffiliatedCompany,
            Remark = contract.Remark
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateContractDto input)
    {
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _contractAppService.UpdateAsync(input);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(input);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel([FromBody] CancelContractDto input)
    {
        try
        {
            await _contractAppService.CancelAsync(input);
            return Json(new { code = 200, msg = "合同已作废" });
        }
        catch (Exception ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GeneratePaymentPlans(int contractId)
    {
        try
        {
            await _contractAppService.GeneratePaymentPlansAsync(contractId);
            return Json(new { code = 200, msg = "回款计划已自动生成" });
        }
        catch (Exception ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddPaymentPlan([FromBody] AddPaymentPlanDto input)
    {
        try
        {
            await _contractAppService.AddPaymentPlanAsync(input);
            return Json(new { code = 200, msg = "回款计划已添加" });
        }
        catch (Exception ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto input)
    {
        try
        {
            await _contractAppService.RecordPaymentAsync(input);
            return Json(new { code = 200, msg = "回款登记成功" });
        }
        catch (Exception ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetContactsByCustomerId(int customerId)
    {
        var contacts = await _contractAppService.GetContactsByCustomerIdAsync(customerId);
        return Json(new { code = 200, data = contacts });
    }
}
