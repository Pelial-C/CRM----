namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerQueryDto
{
    public string? Keyword { get; set; }
    public string? Industry { get; set; }
    public bool IncludeDeleted { get; set; } = false;
}
