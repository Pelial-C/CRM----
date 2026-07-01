namespace CRM.Application.Contracts.OperationLogs.Dtos;

public class OperationLogQueryDto
{
    /// <summary>按聚合名称筛选（Customer / Contract）</summary>
    public string? EntityName { get; set; }

    /// <summary>按聚合根 Id 筛选</summary>
    public int? EntityId { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? EndDate { get; set; }

    /// <summary>页码（从 1 开始）</summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>每页条数</summary>
    public int PageSize { get; set; } = 20;
}
