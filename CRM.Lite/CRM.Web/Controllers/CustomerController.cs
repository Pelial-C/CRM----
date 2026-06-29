using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerAppService _customerAppService;

    public CustomerController(ICustomerAppService customerAppService)
    {
        _customerAppService = customerAppService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? name, string? industry, int pageIndex = 1, int pageSize = 10)
    {
        var query = new CustomerQueryDto
        {
            Name = name,
            Industry = industry,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await _customerAppService.GetPagedListAsync(query);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var customer = await _customerAppService.GetDetailAsync(id);
        return View(customer);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerDto input)
    {
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _customerAppService.CreateAsync(input);
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
        var customer = await _customerAppService.GetByIdAsync(id);
        var dto = new UpdateCustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreditCode = customer.CreditCode,
            Industry = customer.Industry,
            Province = customer.Province,
            City = customer.City,
            District = customer.District,
            DetailAddress = customer.DetailAddress,
            Remark = customer.Remark
        };
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateCustomerDto input)
    {
        if (!ModelState.IsValid) return View(input);

        try
        {
            await _customerAppService.UpdateAsync(input);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(input);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerAppService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
