namespace CRM.Application.Contracts.Contracts.Dtos;

/// <summary>
/// 单月现金流预测数据
/// </summary>
public class CashFlowForecastDto
{
    /// <summary>年份</summary>
    public int Year { get; set; }

    /// <summary>月份</summary>
    public int Month { get; set; }

    /// <summary>月份标签（如 "2026-07"）</summary>
    public string MonthLabel { get; set; } = string.Empty;

    /// <summary>计划回款总额</summary>
    public decimal PlanAmount { get; set; }

    /// <summary>实际回款总额</summary>
    public decimal ActualAmount { get; set; }

    /// <summary>差额（实际 - 计划）</summary>
    public decimal Difference => ActualAmount - PlanAmount;

    /// <summary>达成率（实际 / 计划）</summary>
    public double AchievementRate => PlanAmount > 0 ? (double)(ActualAmount / PlanAmount) : 0;
}
