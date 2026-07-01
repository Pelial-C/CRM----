using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Web.Areas.Enterprise.Controllers;

[Area("Enterprise")]
[Authorize(Roles = "Admin,EnterpriseUser")]
public class DashboardController : Controller
{
    private readonly CrmDbContext _dbContext;

    public DashboardController(CrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var model = new EnterpriseDashboardViewModel
        {
            CustomerCount = await _dbContext.Customers.CountAsync(c => !c.IsDeleted),
            ContractCount = await _dbContext.Contracts.CountAsync(),
            PaymentPlanCount = await _dbContext.PaymentPlans.CountAsync()
        };
        return View(model);
    }
}

public class EnterpriseDashboardViewModel
{
    public int CustomerCount { get; set; }
    public int ContractCount { get; set; }
    public int PaymentPlanCount { get; set; }
}
