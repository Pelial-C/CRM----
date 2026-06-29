using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class ContractItemDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }

    [Required(ErrorMessage = "产品或服务名称不能为空")]
    [MaxLength(100, ErrorMessage = "产品或服务名称不能超过100个字符")]
    public string? ProductName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "数量必须大于0")]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "999999999999", ErrorMessage = "单价必须大于等于0")]
    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }
}
