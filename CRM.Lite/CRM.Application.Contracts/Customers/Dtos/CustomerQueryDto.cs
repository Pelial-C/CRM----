using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerQueryDto
{
    [MaxLength(100, ErrorMessage = "关键字不能超过100个字符")]
    public string? Keyword { get; set; }

    [MaxLength(50, ErrorMessage = "行业不能超过50个字符")]
    public string? Industry { get; set; }

    public bool IncludeDeleted { get; set; } = false;

    [Range(1, int.MaxValue, ErrorMessage = "负责人用户ID无效")]
    public int? OwnerUserId { get; set; }
}
