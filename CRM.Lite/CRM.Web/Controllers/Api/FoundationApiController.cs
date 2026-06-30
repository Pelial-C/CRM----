using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;
using CRM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers.Api;

[Route("api/foundation")]
public class FoundationApiController : ApiControllerBase
{
    [HttpGet("enums")]
    public ActionResult<ApiResponse<FoundationEnumsDto>> GetEnums()
    {
        var data = new FoundationEnumsDto
        {
            ContractStatuses = new()
            {
                new(ContractStatus.Draft, "草稿"),
                new(ContractStatus.Executing, "执行中"),
                new(ContractStatus.Completed, "已完成"),
                new(ContractStatus.Cancelled, "已作废"),
                new(ContractStatus.Terminated, "已终止")
            },
            PaymentPlanStatuses = new()
            {
                new(PaymentPlanStatus.Pending, "未回款"),
                new(PaymentPlanStatus.PartialPaid, "部分回款"),
                new(PaymentPlanStatus.Paid, "已结清"),
                new(PaymentPlanStatus.Overdue, "已逾期")
            },
            PaymentFrequencies = new()
            {
                new(PaymentFrequency.Monthly, "按月"),
                new(PaymentFrequency.Quarterly, "按季度"),
                new(PaymentFrequency.HalfYearly, "按半年"),
                new(PaymentFrequency.Yearly, "按年")
            },
            ServiceTypes = new()
            {
                new(ServiceType.Software, "软件销售"),
                new(ServiceType.Implementation, "实施服务"),
                new(ServiceType.Maintenance, "维保服务"),
                new(ServiceType.Consulting, "咨询服务")
            },
            ContractTypes = new()
            {
                new(ContractType.NewSign, "新签"),
                new(ContractType.Renewal, "续签")
            },
            CustomerIndustries = new()
            {
                new(1, "制造业"),
                new(2, "信息技术"),
                new(3, "金融服务"),
                new(4, "教育培训"),
                new(5, "医疗健康"),
                new(6, "其他")
            },
            ContactTitles = new()
            {
                new(1, "总经理"),
                new(2, "采购负责人"),
                new(3, "财务负责人"),
                new(4, "技术负责人"),
                new(5, "项目联系人")
            }
        };

        return OkResponse(data);
    }
}

public class FoundationEnumsDto
{
    public List<EnumItemDto> ContractStatuses { get; set; } = new();
    public List<EnumItemDto> PaymentPlanStatuses { get; set; } = new();
    public List<EnumItemDto> PaymentFrequencies { get; set; } = new();
    public List<EnumItemDto> ServiceTypes { get; set; } = new();
    public List<EnumItemDto> ContractTypes { get; set; } = new();
    public List<EnumItemDto> CustomerIndustries { get; set; } = new();
    public List<EnumItemDto> ContactTitles { get; set; } = new();
}

public class EnumItemDto
{
    public EnumItemDto(int value, string label)
    {
        Value = value;
        Label = label;
    }

    public EnumItemDto(Enum value, string label)
        : this(Convert.ToInt32(value), label)
    {
    }

    public int Value { get; set; }
    public string Label { get; set; }
}
