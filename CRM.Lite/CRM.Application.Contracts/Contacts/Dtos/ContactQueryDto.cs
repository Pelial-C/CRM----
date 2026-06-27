using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contacts.Dtos;

public class ContactQueryDto
{
    [Required(ErrorMessage = "客户Id不能为空")]
    public int CustomerId { get; set; }

    public string? Name { get; set; }
    public string? Title { get; set; }
}
