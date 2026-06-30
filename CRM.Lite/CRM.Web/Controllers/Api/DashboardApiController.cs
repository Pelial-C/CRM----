using CRM.Domain.Contracts;
using CRM.Infrastructure.Persistence;
using CRM.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Web.Controllers.Api;

[Route("api/dashboard")]
public class DashboardApiController : ApiControllerBase
{
    private readonly CrmDbContext _dbContext;

    public DashboardApiController(CrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary()
    {
        var today = DateTime.Today;
        var dueDate = today.AddDays(30);

        var totalAmount = await _dbContext.Contracts
            .Select(c => (decimal?)c.TotalAmount)
            .SumAsync() ?? 0;

        var paidAmount = await _dbContext.PaymentPlans
            .Select(p => (decimal?)p.ActualAmount)
            .SumAsync() ?? 0;

        var summary = new DashboardSummaryDto
        {
            CustomerCount = await _dbContext.Customers.CountAsync(c => !c.IsDeleted),
            ContactCount = await _dbContext.Contacts.CountAsync(),
            ContractCount = await _dbContext.Contracts.CountAsync(),
            ExecutingContractCount = await _dbContext.Contracts.CountAsync(c => c.Status == ContractStatus.Executing),
            CompletedContractCount = await _dbContext.Contracts.CountAsync(c => c.Status == ContractStatus.Completed),
            CancelledContractCount = await _dbContext.Contracts.CountAsync(c => c.Status == ContractStatus.Cancelled),
            TotalContractAmount = totalAmount,
            PaidAmount = paidAmount,
            PendingAmount = Math.Max(totalAmount - paidAmount, 0),
            OverduePaymentPlanCount = await _dbContext.PaymentPlans.CountAsync(p => p.Status == PaymentPlanStatus.Overdue),
            ContractsDueIn30DaysCount = await _dbContext.Contracts.CountAsync(c =>
                c.Status == ContractStatus.Executing &&
                c.EndDate >= today &&
                c.EndDate <= dueDate)
        };

        return OkResponse(summary);
    }
}

public class DashboardSummaryDto
{
    public int CustomerCount { get; set; }
    public int ContactCount { get; set; }
    public int ContractCount { get; set; }
    public int ExecutingContractCount { get; set; }
    public int CompletedContractCount { get; set; }
    public int CancelledContractCount { get; set; }
    public decimal TotalContractAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal PendingAmount { get; set; }
    public int OverduePaymentPlanCount { get; set; }
    public int ContractsDueIn30DaysCount { get; set; }
}
