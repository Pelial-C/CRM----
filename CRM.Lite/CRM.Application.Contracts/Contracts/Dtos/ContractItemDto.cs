using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class ContractItemDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }

    [Required(ErrorMessage = "产品或服务名称不能为空")]
    [MaxLength(100, ErrorMessage = "产品或服务名称不能超过100个字符")]
    public string? ProductName { get; set; }

    [Required(ErrorMessage = "数量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "单价不能为空")]
    [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "单价必须在0.01到999999999.99之间")]
    public decimal UnitPrice { get; set; }

    [Range(typeof(decimal), "0", "999999999.99", ErrorMessage = "小计金额不能小于0")]
    public decimal Subtotal { get; set; }

    [MaxLength(200, ErrorMessage = "明细备注不能超过200个字符")]
    public string? Remark { get; set; }

    public decimal LineAmount => Quantity * UnitPrice;
}
