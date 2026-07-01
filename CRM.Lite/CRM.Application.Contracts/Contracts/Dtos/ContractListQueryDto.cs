using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class ContractListQueryDto : IValidatableObject
{
    [MaxLength(100, ErrorMessage = "关键字不能超过100个字符")]
    public string? Keyword { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "合同状态无效")]
    public int? Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "客户ID无效")]
    public int? CustomerId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "负责人用户ID无效")]
    public int? OwnerUserId { get; set; }

    [MaxLength(50, ErrorMessage = "合同编号不能超过50个字符")]
    public string? ContractNo { get; set; }

    [MaxLength(100, ErrorMessage = "客户名称不能超过100个字符")]
    public string? CustomerName { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartDateFrom { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartDateTo { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDateFrom { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDateTo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于等于1")]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "每页条数必须在1到100之间")]
    public int PageSize { get; set; } = 20;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDateFrom.HasValue && StartDateTo.HasValue && StartDateFrom > StartDateTo)
        {
            yield return new ValidationResult("开始日期范围不正确", new[] { nameof(StartDateFrom), nameof(StartDateTo) });
        }

        if (EndDateFrom.HasValue && EndDateTo.HasValue && EndDateFrom > EndDateTo)
        {
            yield return new ValidationResult("结束日期范围不正确", new[] { nameof(EndDateFrom), nameof(EndDateTo) });
        }
    }
}
