using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contacts.Dtos;

public class UpdateContactDto : CreateContactDto
{
    [Required(ErrorMessage = "联系人ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "联系人ID无效")]
    public int Id { get; set; }
}
