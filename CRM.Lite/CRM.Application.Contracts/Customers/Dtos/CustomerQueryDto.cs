namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerQueryDto
{
    public string? Keyword { get; set; }
    public bool IncludeDeleted { get; set; } = false;
}
