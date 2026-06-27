using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contacts.Dtos;

public class CreateContactDto
{
    [Required(ErrorMessage = "客户Id不能为空")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "联系人姓名不能为空")]
    [MaxLength(50, ErrorMessage = "联系人姓名不能超过50个字符")]
    public string? Name { get; set; }

    [MaxLength(50, ErrorMessage = "职务不能超过50个字符")]
    public string? Title { get; set; }

    [Phone(ErrorMessage = "手机号格式不正确")]
    [MaxLength(30, ErrorMessage = "手机号不能超过30个字符")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    public string? Email { get; set; }

    public bool IsKeyDecisionMaker { get; set; }
}
