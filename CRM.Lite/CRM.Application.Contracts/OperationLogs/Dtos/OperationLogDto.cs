namespace CRM.Application.Contracts.OperationLogs.Dtos;

public class OperationLogDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    public DateTime OccurredAt { get; set; }
}
