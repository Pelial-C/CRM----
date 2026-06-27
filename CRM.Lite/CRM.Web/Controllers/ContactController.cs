using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers;

public class ContactController : Controller
{
    private readonly IContactAppService _contactAppService;

    public ContactController(IContactAppService contactAppService)
    {
        _contactAppService = contactAppService;
    }

    [HttpGet]
    public async Task<IActionResult> List(int customerId, string? name, string? title)
    {
        var query = new ContactQueryDto
        {
            CustomerId = customerId,
            Name = name,
            Title = title
        };

        var contacts = await _contactAppService.GetListAsync(query);
        ViewBag.CustomerId = customerId;
        return View(contacts);
    }

    [HttpGet]
    public IActionResult Create(int customerId)
    {
        var dto = new CreateContactDto { CustomerId = customerId };
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateContactDto input)
    {
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _contactAppService.CreateAsync(input);
            return RedirectToAction(nameof(List), new { customerId = input.CustomerId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(input);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, int customerId)
    {
        var contact = await _contactAppService.GetByIdAsync(customerId, id);
        var dto = new UpdateContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            Title = contact.Title,
            Phone = contact.Phone,
            Email = contact.Email,
            IsKeyDecisionMaker = contact.IsKeyDecisionMaker
        };
        ViewBag.CustomerId = customerId;
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateContactDto input, int customerId)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CustomerId = customerId;
            return View(input);
        }

        try
        {
            await _contactAppService.UpdateAsync(customerId, input);
            return RedirectToAction(nameof(List), new { customerId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.CustomerId = customerId;
            return View(input);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, int customerId)
    {
        await _contactAppService.DeleteAsync(customerId, id);
        return RedirectToAction(nameof(List), new { customerId });
    }
}
