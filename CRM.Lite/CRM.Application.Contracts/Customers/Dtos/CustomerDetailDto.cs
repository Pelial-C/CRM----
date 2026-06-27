using CRM.Application.Contracts.Contacts.Dtos;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerDetailDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CreditCode { get; set; }
    public string? Industry { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? DetailAddress { get; set; }
    public string? Remark { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreationTime { get; set; }

    public List<ContactDto> Contacts { get; set; } = new();
    public List<ContractSummaryDto> Contracts { get; set; } = new();
}

public class ContractSummaryDto
{
    public int Id { get; set; }
    public string? ContractNo { get; set; }
    public string? ContractName { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public DateTime SignDate { get; set; }
}
