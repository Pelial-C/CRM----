using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;

namespace CRM.Domain.Customers;

/// <summary>
/// 客户等级评估领域服务。
/// 跨聚合计算：需要 Customer + Contract + PaymentPlan 的数据。
/// 评分维度：合同金额（40分）+ 回款及时性（40分）+ 满意度（20分）。
/// </summary>
public class CustomerEvaluationService
{
    /// <summary>
    /// 评估客户等级
    /// </summary>
    /// <param name="customer">客户聚合根</param>
    /// <param name="contracts">该客户的所有合同</param>
    /// <returns>评估结果（三维度得分 + 等级 + 建议）</returns>
    public CustomerEvaluationResult Evaluate(
        Customer customer,
        IReadOnlyCollection<Contract> contracts)
    {
        var contractAmountScore = CalculateContractAmountScore(contracts);
        var paymentTimelinessScore = CalculatePaymentTimelinessScore(contracts);
        var satisfactionScore = CalculateSatisfactionScore();

        return new CustomerEvaluationResult(
            contractAmountScore,
            paymentTimelinessScore,
            satisfactionScore);
    }

    /// <summary>
    /// 合同金额维度（满分 40 分）
    /// 基于该客户所有合同的累计总金额（含执行中和已完成）
    /// </summary>
    private static int CalculateContractAmountScore(IReadOnlyCollection<Contract> contracts)
    {
        // 只统计执行中和已完成的合同（排除作废和终止）
        var activeContracts = contracts
            .Where(c => c.Status == ContractStatus.Executing || c.Status == ContractStatus.Completed)
            .ToList();

        if (activeContracts.Count == 0) return 0;

        var totalAmount = activeContracts.Sum(c => c.TotalAmount);

        // 评分区间（单位：元）
        if (totalAmount >= 1_000_000) return 40;   // 100万以上 → 满分
        if (totalAmount >= 500_000) return 32;     // 50万以上 → 80%
        if (totalAmount >= 100_000) return 24;     // 10万以上 → 60%
        if (totalAmount >= 50_000) return 16;      // 5万以上 → 40%
        if (totalAmount >= 10_000) return 8;       // 1万以上 → 20%
        return 4;                                   // 1万以下 → 最低分
    }

    /// <summary>
    /// 回款及时性维度（满分 40 分）
    /// 基于已结清回款计划中，按时付款的比例
    /// </summary>
    private static int CalculatePaymentTimelinessScore(IReadOnlyCollection<Contract> contracts)
    {
        var allPlans = contracts
            .SelectMany(c => c.PaymentPlans)
            .Where(p => p.Status == PaymentPlanStatus.Paid || p.Status == PaymentPlanStatus.Overdue)
            .ToList();

        if (allPlans.Count == 0) return 10; // 没有回款记录给基础分

        var onTimeCount = allPlans.Count(p =>
            p.Status == PaymentPlanStatus.Paid &&
            p.ActualDate.HasValue &&
            p.ActualDate.Value <= p.PlanDate);

        var ratio = (double)onTimeCount / allPlans.Count;

        // 按时率 → 得分
        if (ratio >= 0.9) return 40;   // 90%以上 → 满分
        if (ratio >= 0.7) return 32;   // 70%以上 → 80%
        if (ratio >= 0.5) return 24;   // 50%以上 → 60%
        if (ratio >= 0.3) return 16;   // 30%以上 → 40%
        return 8;                       // 30%以下 → 20%
    }

    /// <summary>
    /// 满意度维度（满分 20 分）
    /// 当前版本：默认给 15 分（良好）
    /// 后续可扩展为：客户反馈评分、投诉次数等
    /// </summary>
    private static int CalculateSatisfactionScore()
    {
        // TODO: 后续可从客户反馈表、投诉记录等计算
        return 15;
    }
}
