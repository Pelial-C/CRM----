using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerQueryDto
{
    public string? Name { get; set; }
    public string? Industry { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "每页条数必须在1-100之间")]
    public int PageSize { get; set; } = 10;
}
