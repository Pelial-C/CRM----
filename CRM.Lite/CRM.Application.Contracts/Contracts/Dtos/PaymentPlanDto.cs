using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class PaymentPlanDto
{
    public int Id { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "合同ID无效")]
    public int ContractId { get; set; }
    [DataType(DataType.Date)]
    public DateTime PlanDate { get; set; }
    [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "计划回款金额必须在0.01到999999999.99之间")]
    public decimal PlanAmount { get; set; }
    [Range(typeof(decimal), "0", "999999999.99", ErrorMessage = "实际回款金额不能小于0")]
    public decimal ActualAmount { get; set; }
    [DataType(DataType.Date)]
    public DateTime? ActualDate { get; set; }
    public int Status { get; set; }
    [MaxLength(200, ErrorMessage = "说明不能超过200个字符")]
    public string? Description { get; set; }
}
