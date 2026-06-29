namespace CRM.Application.Contracts.Contracts.Dtos;

public class PagedResultDto<T>
{
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = new();
}
