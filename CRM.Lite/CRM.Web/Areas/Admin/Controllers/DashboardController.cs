using CRM.Domain.Contracts;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly CrmDbContext _dbContext;

    public DashboardController(CrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var dueDate = today.AddDays(30);
        var model = new AdminDashboardViewModel
        {
            CustomerCount = await _dbContext.Customers.CountAsync(c => !c.IsDeleted),
            ContractCount = await _dbContext.Contracts.CountAsync(),
            PendingPaymentPlanCount = await _dbContext.PaymentPlans.CountAsync(p => p.Status != PaymentPlanStatus.Paid),
            ContractsDueIn30DaysCount = await _dbContext.Contracts.CountAsync(c =>
                c.Status == ContractStatus.Executing &&
                c.EndDate >= today &&
                c.EndDate <= dueDate),
            UserName = User.Identity?.Name ?? "-",
            Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "-"
        };

        return View(model);
    }
}

public class AdminDashboardViewModel
{
    public int CustomerCount { get; set; }
    public int ContractCount { get; set; }
    public int PendingPaymentPlanCount { get; set; }
    public int ContractsDueIn30DaysCount { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
