using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class RecordPaymentDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ContractId必须大于0")]
    public int ContractId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PlanId必须大于0")]
    public int PlanId { get; set; }

    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "实际回款金额必须大于0")]
    public decimal ActualAmount { get; set; }

    [Required(ErrorMessage = "实际回款日期不能为空")]
    public DateTime ActualDate { get; set; } = DateTime.Today;
}
