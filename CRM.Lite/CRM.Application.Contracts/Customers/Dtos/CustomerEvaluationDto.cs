using CRM.Domain.Shared.Enums;

namespace CRM.Application.Contracts.Customers.Dtos;

/// <summary>
/// 客户评估结果 DTO
/// </summary>
public class CustomerEvaluationDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public CustomerLevel Level { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int ContractAmountScore { get; set; }
    public int PaymentTimelinessScore { get; set; }
    public int SatisfactionScore { get; set; }
    public string Suggestion { get; set; } = string.Empty;
}
