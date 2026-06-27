namespace CRM.Application.Contracts.Contracts.Dtos;

public class ContractDto
{
    public int Id { get; set; }
    public string? ContractNo { get; set; }
    public string? ContractName { get; set; }
    public string? CabinetNo { get; set; }
    public DateTime SignDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public int ServiceType { get; set; }
    public int ContractType { get; set; }
    public int PaymentFrequency { get; set; }
    public int WarningDays { get; set; }
    public string? RegionalCompany { get; set; }
    public string? AffiliatedCompany { get; set; }
    public DateTime CreationTime { get; set; }
    public string? Remark { get; set; }
    public List<ContractItemDto> Items { get; set; } = new();
    public List<PaymentPlanDto> PaymentPlans { get; set; } = new();
}
