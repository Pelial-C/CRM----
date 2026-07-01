using System.ComponentModel.DataAnnotations;
using CRM.Domain.Shared.Enums;

namespace CRM.Web.Models.AdminUsers;

public class EditUserViewModel
{
    [Required(ErrorMessage = "用户ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "用户ID无效")]
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "显示名称不能为空")]
    [MaxLength(50, ErrorMessage = "显示名称不能超过50个字符")]
    public string? DisplayName { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }

    [MaxLength(30, ErrorMessage = "手机号不能超过30个字符")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "角色不能为空")]
    public UserRole Role { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "关联客户ID无效")]
    public int? RelatedCustomerId { get; set; }

    public bool IsActive { get; set; }
}
