using System.ComponentModel.DataAnnotations;
using CRM.Domain.Shared.Enums;

namespace CRM.Web.Models.AdminUsers;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "显示名称不能为空")]
    [MaxLength(50, ErrorMessage = "显示名称不能超过50个字符")]
    public string? DisplayName { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }

    [MaxLength(30, ErrorMessage = "手机号不能超过30个字符")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "角色不能为空")]
    public UserRole Role { get; set; } = UserRole.EnterpriseUser;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能少于6位")]
    [MaxLength(100, ErrorMessage = "密码不能超过100个字符")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "关联客户ID无效")]
    public int? RelatedCustomerId { get; set; }

    public bool IsActive { get; set; } = true;
}
