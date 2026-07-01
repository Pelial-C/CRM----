using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Application.Contracts.Customers;
using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Web.Controllers;

[Authorize(Roles = "Admin,Sales,EnterpriseUser")]
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
            OwnerUserId = User.IsInRole("Sales") ? GetCurrentUserId() : null,
            PageIndex = pageIndex,
            PageSize = 20
        });

        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        return View(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new CreateContractDto
        {
            SignDate = DateTime.Today,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(12),
            WarningDays = 30,
            OwnerUserId = User.IsInRole("Sales") ? GetCurrentUserId() : null,
            Items = new List<ContractItemDto> { new() { Quantity = 1 } }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Create(CreateContractDto input)
    {
        if (User.IsInRole("Sales")) input.OwnerUserId = GetCurrentUserId();
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
    [Authorize(Roles = "Admin,Sales")]
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
            OwnerUserId = contract.OwnerUserId,
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
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Edit(UpdateContractDto input)
    {
        if (User.IsInRole("Sales")) input.OwnerUserId = GetCurrentUserId();
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancel(int id, string? reason)
    {
        await _contractAppService.CancelAsync(id, reason);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> GeneratePaymentPlans(int contractId)
    {
        try
        {
            await _contractAppService.GeneratePaymentPlansAsync(contractId);
        }
        catch (BusinessException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Detail), new { id = contractId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> AddPaymentPlan(AddPaymentPlanDto input)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _contractAppService.AddPaymentPlanAsync(input);
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }
        }
        else
        {
            TempData["Error"] = "回款计划参数校验失败";
        }

        return RedirectToAction(nameof(Detail), new { id = input.ContractId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> RecordPayment(RecordPaymentDto input)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _contractAppService.RecordPaymentAsync(input);
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
            }
        }
        else
        {
            TempData["Error"] = "登记回款参数校验失败";
        }

        return RedirectToAction(nameof(Detail), new { id = input.ContractId });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> GetContactsByCustomerId(int customerId)
    {
        var contacts = await _contractAppService.GetContactsByCustomerIdAsync(customerId);
        return Json(contacts);
    }

    private async Task LoadLookupsAsync(int? customerId = null)
    {
        if (User.IsInRole("Sales"))
        {
            var ownerUserId = GetCurrentUserId();
            var customers = await _customerAppService.GetListAsync(new CRM.Application.Contracts.Customers.Dtos.CustomerQueryDto
            {
                OwnerUserId = ownerUserId
            });
            ViewBag.Customers = customers.Select(c => new CRM.Application.Contracts.Customers.Dtos.CustomerSelectDto
            {
                Id = c.Id,
                Name = c.Name ?? string.Empty
            }).ToList();
        }
        else
        {
            ViewBag.Customers = await _customerAppService.GetSelectListAsync();
        }
        ViewBag.Contacts = customerId.HasValue && customerId.Value > 0
            ? await _contractAppService.GetContactsByCustomerIdAsync(customerId.Value)
            : new List<ContactSelectDto>();
        ViewBag.ContractStatuses = Enum.GetValues<ContractStatus>();
        ViewBag.PaymentFrequencies = Enum.GetValues<PaymentFrequency>();
        ViewBag.ServiceTypes = Enum.GetValues<ServiceType>();
        ViewBag.ContractTypes = Enum.GetValues<ContractType>();
    }

    private int? GetCurrentUserId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
    }
}
