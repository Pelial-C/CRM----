using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "企业名称不能为空")]
    [MaxLength(100, ErrorMessage = "企业名称不能超过100个字符")]
    public string? Name { get; set; }

    [MaxLength(50, ErrorMessage = "统一社会信用代码不能超过50个字符")]
    [RegularExpression(@"^[0-9A-Z]{18}$", ErrorMessage = "统一社会信用代码格式不正确")]
    public string? CreditCode { get; set; }

    [MaxLength(50, ErrorMessage = "行业不能超过50个字符")]
    public string? Industry { get; set; }

    [MaxLength(50, ErrorMessage = "省份不能超过50个字符")]
    public string? Province { get; set; }

    [MaxLength(50, ErrorMessage = "城市不能超过50个字符")]
    public string? City { get; set; }

    [MaxLength(50, ErrorMessage = "区县不能超过50个字符")]
    public string? District { get; set; }

    [MaxLength(200, ErrorMessage = "详细地址不能超过200个字符")]
    public string? DetailAddress { get; set; }

    [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
    public string? Remark { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "负责人用户ID无效")]
    public int? OwnerUserId { get; set; }
}
