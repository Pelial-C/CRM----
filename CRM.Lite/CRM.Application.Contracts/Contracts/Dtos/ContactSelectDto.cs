namespace CRM.Application.Contracts.Contracts.Dtos;

public class ContactSelectDto
{
    public int ContactId { get; set; }
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
