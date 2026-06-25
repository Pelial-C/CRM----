namespace CRM.Application.Contracts.Contracts.Dtos;

public class ContractItemDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
