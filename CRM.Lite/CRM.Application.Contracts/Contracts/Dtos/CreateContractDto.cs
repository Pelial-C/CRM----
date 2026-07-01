using System.ComponentModel.DataAnnotations;
using CRM.Domain.Shared.Enums;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class CreateContractDto : IValidatableObject
{
    private const decimal AmountTolerance = 0.01m;

    [Required(ErrorMessage = "合同编号不能为空")]
    [MaxLength(50, ErrorMessage = "合同编号不能超过50个字符")]
    public string? ContractNo { get; set; }

    [Required(ErrorMessage = "合同名称不能为空")]
    [MaxLength(100, ErrorMessage = "合同名称不能超过100个字符")]
    public string? ContractName { get; set; }

    [MaxLength(50, ErrorMessage = "合同柜编号不能超过50个字符")]
    public string? CabinetNo { get; set; }

    [Required(ErrorMessage = "签订日期不能为空")]
    [DataType(DataType.Date)]
    public DateTime SignDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "开始日期不能为空")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "结束日期不能为空")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "合同总金额不能为空")]
    [Range(typeof(decimal), "0.01", "999999999.99", ErrorMessage = "合同总金额必须在0.01到999999999.99之间")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "客户不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "客户ID无效")]
    public int CustomerId { get; set; }

    [MaxLength(100, ErrorMessage = "客户名称不能超过100个字符")]
    public string? CustomerName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "负责人用户ID无效")]
    public int? OwnerUserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "联系人ID无效")]
    public int? ContactId { get; set; }

    [MaxLength(50, ErrorMessage = "联系人名称不能超过50个字符")]
    public string? ContactName { get; set; }

    [Required(ErrorMessage = "回款频率不能为空")]
    [EnumDataType(typeof(PaymentFrequency), ErrorMessage = "回款频率无效")]
    public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;

    [Required(ErrorMessage = "服务类型不能为空")]
    [EnumDataType(typeof(ServiceType), ErrorMessage = "服务类型无效")]
    public ServiceType ServiceType { get; set; } = ServiceType.Software;

    [Required(ErrorMessage = "合同类型不能为空")]
    [EnumDataType(typeof(ContractType), ErrorMessage = "合同类型无效")]
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
    public List<PaymentPlanDto> PaymentPlans { get; set; } = new();

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult("结束日期不能早于开始日期", new[] { nameof(EndDate) });
        }

        if (SignDate > EndDate)
        {
            yield return new ValidationResult("签订日期不能晚于结束日期", new[] { nameof(SignDate) });
        }

        if (WarningDays < 0)
        {
            yield return new ValidationResult("预警天数不能小于0", new[] { nameof(WarningDays) });
        }

        if (Items is { Count: > 0 })
        {
            var filledItems = Items.Where(i => !string.IsNullOrWhiteSpace(i.ProductName)).ToList();
            var itemTotal = filledItems.Sum(i => i.Quantity * i.UnitPrice);
            if (filledItems.Count > 0 && Math.Abs(itemTotal - TotalAmount) > AmountTolerance)
            {
                yield return new ValidationResult("合同明细合计金额应与合同总金额一致，允许0.01元以内误差", new[] { nameof(Items), nameof(TotalAmount) });
            }
        }

        if (PaymentPlans is { Count: > 0 })
        {
            var planTotal = PaymentPlans.Sum(p => p.PlanAmount);
            if (planTotal > TotalAmount + AmountTolerance)
            {
                yield return new ValidationResult("计划回款总额不能超过合同总金额", new[] { nameof(PaymentPlans), nameof(TotalAmount) });
            }
        }
    }
}
