using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contacts.Dtos;

public class ContactDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsKeyDecisionMaker { get; set; }
}

public class CreateContactDto
{
    [Required(ErrorMessage = "联系人姓名不能为空")]
    [MaxLength(50, ErrorMessage = "联系人姓名不能超过50个字符")]
    public string? Name { get; set; }

    [MaxLength(50, ErrorMessage = "职务不能超过50个字符")]
    public string? Title { get; set; }

    [MaxLength(30, ErrorMessage = "手机号不能超过30个字符")]
    public string? Phone { get; set; }

    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    public bool IsKeyDecisionMaker { get; set; }
}

public class UpdateContactDto : CreateContactDto
{
    [Range(1, int.MaxValue, ErrorMessage = "联系人Id必须大于0")]
    public int Id { get; set; }
}
