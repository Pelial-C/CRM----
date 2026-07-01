using CRM.Domain.Contracts;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Web.Areas.Sales.Controllers;

[Area("Sales")]
[Authorize(Roles = "Admin,Sales")]
public class DashboardController : Controller
{
    private readonly CrmDbContext _dbContext;

    public DashboardController(CrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var parsed) ? parsed : 0;
        var isAdmin = User.IsInRole("Admin");
        var contracts = _dbContext.Contracts.AsQueryable();
        var customers = _dbContext.Customers.AsQueryable();
        if (!isAdmin && userId > 0)
        {
            contracts = contracts.Where(c => c.OwnerUserId == userId);
            customers = customers.Where(c => c.OwnerUserId == userId);
        }

        var model = new SalesDashboardViewModel
        {
            CustomerCount = await customers.CountAsync(c => !c.IsDeleted),
            ContractCount = await contracts.CountAsync(),
            ExecutingContractCount = await contracts.CountAsync(c => c.Status == ContractStatus.Executing),
            DueSoonContractCount = await contracts.CountAsync(c => c.Status == ContractStatus.Executing && c.EndDate <= DateTime.Today.AddDays(30))
        };
        return View(model);
    }
}

public class SalesDashboardViewModel
{
    public int CustomerCount { get; set; }
    public int ContractCount { get; set; }
    public int ExecutingContractCount { get; set; }
    public int DueSoonContractCount { get; set; }
}
