using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Services;

/// <summary>
/// 现金流预测服务 —— 按月份聚合回款计划的计划金额与实际金额。
/// 纯查询服务，不涉及领域规则，直接通过 DbContext 查询。
/// </summary>
public class CashFlowForecastService : ICashFlowForecastService
{
    private readonly CrmDbContext _dbContext;

    public CashFlowForecastService(CrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CashFlowForecastDto>> GetForecastAsync(int months = 12)
    {
        if (months <= 0 || months > 36) months = 12;

        var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var endDate = startDate.AddMonths(months);

        // 查询时间范围内的所有回款计划，按月分组聚合
        var monthlyData = await _dbContext.PaymentPlans
            .Where(p => p.PlanDate >= startDate && p.PlanDate < endDate)
            .GroupBy(p => new { p.PlanDate.Year, p.PlanDate.Month })
            .Select(g => new CashFlowForecastDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthLabel = $"{g.Key.Year}-{g.Key.Month:D2}",
                PlanAmount = g.Sum(p => p.PlanAmount),
                ActualAmount = g.Sum(p => p.ActualAmount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        // 补全没有数据的月份（确保图表连续）
        var result = new List<CashFlowForecastDto>();
        var current = startDate;

        while (current < endDate)
        {
            var existing = monthlyData.FirstOrDefault(d => d.Year == current.Year && d.Month == current.Month);

            if (existing != null)
            {
                result.Add(existing);
            }
            else
            {
                result.Add(new CashFlowForecastDto
                {
                    Year = current.Year,
                    Month = current.Month,
                    MonthLabel = $"{current.Year}-{current.Month:D2}",
                    PlanAmount = 0,
                    ActualAmount = 0
                });
            }

            current = current.AddMonths(1);
        }

        return result;
    }
}
