using CRM.Domain.Shared.Enums;

namespace CRM.Domain.Customers;

/// <summary>
/// 客户评估结果 —— 值对象，包含三维度评分和最终等级。
/// 由 CustomerEvaluationService 计算生成。
/// </summary>
public class CustomerEvaluationResult
{
    /// <summary>合同金额维度得分（满分 40）</summary>
    public int ContractAmountScore { get; }

    /// <summary>回款及时性维度得分（满分 40）</summary>
    public int PaymentTimelinessScore { get; }

    /// <summary>满意度维度得分（满分 20）</summary>
    public int SatisfactionScore { get; }

    /// <summary>总分（满分 100）</summary>
    public int TotalScore => ContractAmountScore + PaymentTimelinessScore + SatisfactionScore;

    /// <summary>根据总分计算的客户等级</summary>
    public CustomerLevel Level { get; }

    /// <summary>评估建议</summary>
    public string Suggestion { get; }

    public CustomerEvaluationResult(
        int contractAmountScore,
        int paymentTimelinessScore,
        int satisfactionScore)
    {
        ContractAmountScore = contractAmountScore;
        PaymentTimelinessScore = paymentTimelinessScore;
        SatisfactionScore = satisfactionScore;
        Level = CalculateLevel(TotalScore);
        Suggestion = GenerateSuggestion(Level);
    }

    private static CustomerLevel CalculateLevel(int totalScore)
    {
        if (totalScore >= 80) return CustomerLevel.Strategic;
        if (totalScore >= 60) return CustomerLevel.Important;
        if (totalScore >= 40) return CustomerLevel.Normal;
        return CustomerLevel.Risk;
    }

    private static string GenerateSuggestion(CustomerLevel level)
    {
        return level switch
        {
            CustomerLevel.Strategic => "核心战略客户，优先配置资源，保持高层互动频率",
            CustomerLevel.Important => "重点维护客户，加强定期回访，关注续签意向",
            CustomerLevel.Normal => "普通客户，按标准流程维护，关注成长潜力",
            CustomerLevel.Risk => "风险客户，需立即跟进，排查回款异常和满意度问题",
            _ => "暂无评估数据"
        };
    }
}
