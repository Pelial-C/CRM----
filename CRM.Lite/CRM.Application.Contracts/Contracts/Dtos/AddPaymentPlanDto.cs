using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class AddPaymentPlanDto
{
    [Required(ErrorMessage = "合同ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "合同ID无效")]
    public int ContractId { get; set; }

    [Required(ErrorMessage = "计划回款日期不能为空")]
    [DataType(DataType.Date)]
    public DateTime PlanDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "计划回款金额不能为空")]
    [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "计划回款金额必须在0.01到999999999.99之间")]
    public decimal PlanAmount { get; set; }

    [MaxLength(200, ErrorMessage = "说明不能超过200个字符")]
    public string? Description { get; set; }
}
