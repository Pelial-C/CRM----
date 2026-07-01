using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class CancelContractDto
{
    [Required(ErrorMessage = "合同ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "合同ID无效")]
    public int ContractId { get; set; }

    [Required(ErrorMessage = "作废原因不能为空")]
    [MaxLength(300, ErrorMessage = "作废原因不能超过300个字符")]
    public string? CancelReason { get; set; }

    [Required(ErrorMessage = "作废日期不能为空")]
    [DataType(DataType.Date)]
    public DateTime CancelDate { get; set; } = DateTime.Today;
}
