using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Application.Contracts.Customers;
using CRM.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers;

[Authorize(Roles = "Admin,Sales,EnterpriseUser")]
public class ContactController : Controller
{
    private readonly IContactAppService _contactAppService;
    private readonly ICustomerAppService _customerAppService;

    public ContactController(IContactAppService contactAppService, ICustomerAppService customerAppService)
    {
        _contactAppService = contactAppService;
        _customerAppService = customerAppService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? customerId = null)
    {
        await LoadCustomersAsync();
        ViewBag.CustomerId = customerId;
        var contacts = customerId.HasValue && customerId.Value > 0
            ? await _contactAppService.GetListByCustomerIdAsync(customerId.Value)
            : new List<ContactDto>();

        return View(contacts);
    }

    [HttpGet]
    public Task<IActionResult> List(int? customerId = null)
    {
        return Task.FromResult<IActionResult>(RedirectToAction(nameof(Index), new { customerId }));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Create(int customerId)
    {
        await LoadCustomersAsync();
        return View(new CreateContactDto { CustomerId = customerId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Create(CreateContactDto input)
    {
        if (!ModelState.IsValid)
        {
            await LoadCustomersAsync();
            return View(input);
        }

        try
        {
            await _contactAppService.CreateAsync(input.CustomerId, input);
            return RedirectToAction(nameof(Index), new { customerId = input.CustomerId });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCustomersAsync();
            return View(input);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Edit(int id)
    {
        var contact = await _contactAppService.GetAsync(id);
        await LoadCustomersAsync();
        return View(new UpdateContactDto
        {
            Id = contact.Id,
            CustomerId = contact.CustomerId,
            Name = contact.Name,
            Title = contact.Title,
            Phone = contact.Phone,
            Email = contact.Email,
            IsKeyDecisionMaker = contact.IsKeyDecisionMaker
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<IActionResult> Edit(UpdateContactDto input)
    {
        if (!ModelState.IsValid)
        {
            await LoadCustomersAsync();
            return View(input);
        }

        try
        {
            await _contactAppService.UpdateAsync(input);
            return RedirectToAction(nameof(Index), new { customerId = input.CustomerId });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCustomersAsync();
            return View(input);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, int customerId)
    {
        await _contactAppService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { customerId });
    }

    private async Task LoadCustomersAsync()
    {
        if (User.IsInRole("Sales"))
        {
            var userId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var parsed) ? parsed : 0;
            var customers = await _customerAppService.GetListAsync(new CRM.Application.Contracts.Customers.Dtos.CustomerQueryDto
            {
                OwnerUserId = userId > 0 ? userId : null
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
    }
}
