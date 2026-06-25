namespace CRM.Application.Contracts.Contracts.Dtos;

public class PaymentPlanDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public DateTime PlanDate { get; set; }
    public decimal PlanAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
    public int Status { get; set; }
    public string? Description { get; set; }
}
