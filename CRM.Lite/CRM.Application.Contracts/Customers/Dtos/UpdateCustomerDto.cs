using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Customers.Dtos;

public class UpdateCustomerDto : CreateCustomerDto
{
    [Required(ErrorMessage = "客户ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "客户ID无效")]
    public int Id { get; set; }
}
