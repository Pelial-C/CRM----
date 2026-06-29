using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Application.Contracts.Customers;
using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers;

public class ContractController : Controller
{
    private readonly IContractAppService _contractAppService;
    private readonly ICustomerAppService _customerAppService;

    public ContractController(IContractAppService contractAppService, ICustomerAppService customerAppService)
    {
        _contractAppService = contractAppService;
        _customerAppService = customerAppService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword, int? status, int pageIndex = 1)
    {
        var result = await _contractAppService.GetListAsync(new ContractListQueryDto
        {
            Keyword = keyword,
            Status = status,
            PageIndex = pageIndex,
            PageSize = 20
        });

        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new CreateContractDto
        {
            SignDate = DateTime.Today,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(12),
            WarningDays = 30,
            Items = new List<ContractItemDto> { new() { Quantity = 1 } }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateContractDto input)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(input.CustomerId);
            return View(input);
        }

        try
        {
            await _contractAppService.CreateAsync(input);
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadLookupsAsync(input.CustomerId);
            return View(input);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contract = await _contractAppService.GetAsync(id);
        await LoadLookupsAsync(contract.CustomerId);
        return View(new UpdateContractDto
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
            PaymentFrequency = contract.PaymentFrequency,
            ServiceType = contract.ServiceType,
            ContractType = contract.ContractType,
            WarningDays = contract.WarningDays,
            RegionalCompany = contract.RegionalCompany,
            AffiliatedCompany = contract.AffiliatedCompany,
            Remark = contract.Remark,
            Items = contract.Items.Count > 0 ? contract.Items : new List<ContractItemDto> { new() { Quantity = 1 } }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateContractDto input)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(input.CustomerId);
            return View(input);
        }

        try
        {
            await _contractAppService.UpdateAsync(input);
            return RedirectToAction(nameof(Detail), new { id = input.Id });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadLookupsAsync(input.CustomerId);
            return View(input);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var contract = await _contractAppService.GetAsync(id);
        return View(contract);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, string? reason)
    {
        await _contractAppService.CancelAsync(id, reason);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneratePaymentPlans(int contractId)
    {
        await _contractAppService.GeneratePaymentPlansAsync(contractId);
        return RedirectToAction(nameof(Detail), new { id = contractId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPaymentPlan(AddPaymentPlanDto input)
    {
        if (ModelState.IsValid)
        {
            await _contractAppService.AddPaymentPlanAsync(input);
        }

        return RedirectToAction(nameof(Detail), new { id = input.ContractId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordPayment(RecordPaymentDto input)
    {
        if (ModelState.IsValid)
        {
            await _contractAppService.RecordPaymentAsync(input);
        }

        return RedirectToAction(nameof(Detail), new { id = input.ContractId });
    }

    [HttpGet]
    public async Task<IActionResult> GetContactsByCustomerId(int customerId)
    {
        var contacts = await _contractAppService.GetContactsByCustomerIdAsync(customerId);
        return Json(contacts);
    }

    private async Task LoadLookupsAsync(int? customerId = null)
    {
        ViewBag.Customers = await _customerAppService.GetSelectListAsync();
        ViewBag.Contacts = customerId.HasValue && customerId.Value > 0
            ? await _contractAppService.GetContactsByCustomerIdAsync(customerId.Value)
            : new List<ContactSelectDto>();
        ViewBag.ContractStatuses = Enum.GetValues<ContractStatus>();
        ViewBag.PaymentFrequencies = Enum.GetValues<PaymentFrequency>();
        ViewBag.ServiceTypes = Enum.GetValues<ServiceType>();
        ViewBag.ContractTypes = Enum.GetValues<ContractType>();
    }
}
