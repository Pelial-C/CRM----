using CRM.Domain.Shared.Enums;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class CreateContractDto
{
    public string? ContractNo { get; set; }
    public string? ContractName { get; set; }
    public DateTime SignDate { get; set; } = DateTime.Today;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public decimal TotalAmount { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;
    public ServiceType ServiceType { get; set; } = ServiceType.Software;
    public ContractType ContractType { get; set; } = ContractType.NewSign;
    public string? RegionalCompany { get; set; }
    public string? AffiliatedCompany { get; set; }
    public string? Remark { get; set; }
    public List<ContractItemDto> Items { get; set; } = new();
    public List<PaymentPlanDto> PaymentPlans { get; set; } = new();
}
