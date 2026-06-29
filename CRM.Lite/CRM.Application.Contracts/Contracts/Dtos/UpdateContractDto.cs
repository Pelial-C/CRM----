using System.ComponentModel.DataAnnotations;
using CRM.Domain.Shared.Enums;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class UpdateContractDto : IValidatableObject
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "合同编号不能为空")]
    [MaxLength(50, ErrorMessage = "合同编号不能超过50个字符")]
    public string? ContractNo { get; set; }

    [Required(ErrorMessage = "合同名称不能为空")]
    [MaxLength(100, ErrorMessage = "合同名称不能超过100个字符")]
    public string? ContractName { get; set; }

    [MaxLength(50, ErrorMessage = "合同柜编号不能超过50个字符")]
    public string? CabinetNo { get; set; }

    public DateTime SignDate { get; set; } = DateTime.Today;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Range(typeof(decimal), "0.01", "999999999999", ErrorMessage = "合同总金额必须大于0")]
    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "必须选择客户")]
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;
    public ServiceType ServiceType { get; set; } = ServiceType.Software;
    public ContractType ContractType { get; set; } = ContractType.NewSign;

    [Range(0, 365, ErrorMessage = "预警天数必须在0到365之间")]
    public int WarningDays { get; set; } = 30;

    [MaxLength(100, ErrorMessage = "区域公司不能超过100个字符")]
    public string? RegionalCompany { get; set; }

    [MaxLength(100, ErrorMessage = "所属公司不能超过100个字符")]
    public string? AffiliatedCompany { get; set; }

    [MaxLength(500, ErrorMessage = "备注不能超过500个字符")]
    public string? Remark { get; set; }

    public List<ContractItemDto> Items { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult("结束日期不能早于开始日期", new[] { nameof(EndDate) });
        }
    }
}
