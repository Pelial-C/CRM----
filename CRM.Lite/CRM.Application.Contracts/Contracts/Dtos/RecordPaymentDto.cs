using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class RecordPaymentDto
{
    [Required(ErrorMessage = "合同ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "合同ID无效")]
    public int ContractId { get; set; }

    [Required(ErrorMessage = "回款计划ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "回款计划ID无效")]
    public int PlanId { get; set; }

    [Required(ErrorMessage = "实际回款金额不能为空")]
    [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "实际回款金额必须在0.01到999999999.99之间")]
    public decimal ActualAmount { get; set; }

    [Required(ErrorMessage = "实际回款日期不能为空")]
    [DataType(DataType.Date)]
    public DateTime ActualDate { get; set; } = DateTime.Today;

    [MaxLength(200, ErrorMessage = "回款备注不能超过200个字符")]
    public string? Remark { get; set; }
}
