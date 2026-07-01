using CRM.Domain.Shared.Enums;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CreditCode { get; set; }
    public string? Industry { get; set; }
    public string? Remark { get; set; }
    public bool IsDeleted { get; set; }
    public int? OwnerUserId { get; set; }
    public CustomerLevel Level { get; set; }

    // 将值对象 Address 展平为普通字符串，方便前端展示
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? DetailAddress { get; set; }

    public DateTime CreationTime { get; set; }
    public List<CustomerContactDto> Contacts { get; set; } = new();
}

public class CustomerContactDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsKeyDecisionMaker { get; set; }
}
