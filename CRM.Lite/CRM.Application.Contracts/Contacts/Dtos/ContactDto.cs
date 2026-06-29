namespace CRM.Application.Contracts.Contacts.Dtos;

public class ContactDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsKeyDecisionMaker { get; set; }
}
