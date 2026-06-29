using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class AddPaymentPlanDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ContractId必须大于0")]
    public int ContractId { get; set; }

    [Required(ErrorMessage = "计划回款日期不能为空")]
    public DateTime PlanDate { get; set; } = DateTime.Today;

    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "计划回款金额必须大于0")]
    public decimal PlanAmount { get; set; }

    [MaxLength(200, ErrorMessage = "说明不能超过200个字符")]
    public string? Description { get; set; }
}
