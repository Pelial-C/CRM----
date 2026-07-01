using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using CRM.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Web.Controllers;

[Authorize(Roles = "Admin,Sales,EnterpriseUser")]
public class CustomerController : Controller
{
    private readonly ICustomerAppService _customerAppService;

    public CustomerController(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    public IActionResult Create()
    {
        return View(new CreateCustomerDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Create(CreateCustomerDto input)
    {
        if (User.IsInRole("Sales")) input.OwnerUserId = GetCurrentUserId();
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _customerAppService.CreateAsync(input);
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(input);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerAppService.GetByIdAsync(id);
        return View(new UpdateCustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreditCode = customer.CreditCode,
            Industry = customer.Industry,
            Province = customer.Province,
            City = customer.City,
            District = customer.District,
            DetailAddress = customer.DetailAddress,
            Remark = customer.Remark,
            OwnerUserId = customer.OwnerUserId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Edit(UpdateCustomerDto input)
    {
        if (User.IsInRole("Sales")) input.OwnerUserId = GetCurrentUserId();
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _customerAppService.UpdateAsync(input);
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(input);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetList(string? keyword, bool includeDeleted = false)
    {
        var data = await _customerAppService.GetListAsync(new CustomerQueryDto
        {
            Keyword = keyword,
            IncludeDeleted = includeDeleted,
            OwnerUserId = User.IsInRole("Sales") ? GetCurrentUserId() : null
        });
        return Json(new { code = 200, msg = "success", data });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _customerAppService.DeleteAsync(id);
            return Json(new { code = 200, msg = "删除成功" });
        }
        catch (BusinessException ex)
        {
            return Json(new { code = 400, msg = ex.Message });
        }
    }

    private int? GetCurrentUserId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
    }
}
