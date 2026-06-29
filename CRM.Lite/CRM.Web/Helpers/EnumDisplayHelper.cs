using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;

namespace CRM.Web.Helpers;

public static class EnumDisplayHelper
{
    public static string ContractStatusText(int status)
    {
        return Enum.IsDefined(typeof(ContractStatus), status)
            ? ContractStatusText((ContractStatus)status)
            : "未知";
    }

    public static string ContractStatusText(ContractStatus status)
    {
        return status switch
        {
            ContractStatus.Draft => "草稿",
            ContractStatus.Executing => "执行中",
            ContractStatus.Completed => "已完成",
            ContractStatus.Cancelled => "已作废",
            ContractStatus.Terminated => "已终止",
            _ => "未知"
        };
    }

    public static string PaymentPlanStatusText(int status)
    {
        return Enum.IsDefined(typeof(PaymentPlanStatus), status)
            ? PaymentPlanStatusText((PaymentPlanStatus)status)
            : "未知";
    }

    public static string PaymentPlanStatusText(PaymentPlanStatus status)
    {
        return status switch
        {
            PaymentPlanStatus.Pending => "未回款",
            PaymentPlanStatus.PartialPaid => "部分回款",
            PaymentPlanStatus.Paid => "已结清",
            PaymentPlanStatus.Overdue => "已逾期",
            _ => "未知"
        };
    }

    public static string PaymentFrequencyText(PaymentFrequency frequency)
    {
        return frequency switch
        {
            PaymentFrequency.Monthly => "按月",
            PaymentFrequency.Quarterly => "按季度",
            PaymentFrequency.HalfYearly => "按半年",
            PaymentFrequency.Yearly => "按年",
            _ => "未知"
        };
    }

    public static string ServiceTypeText(ServiceType serviceType)
    {
        return serviceType switch
        {
            ServiceType.Software => "软件销售",
            ServiceType.Implementation => "实施服务",
            ServiceType.Maintenance => "维保服务",
            ServiceType.Consulting => "咨询服务",
            _ => "未知"
        };
    }

    public static string ContractTypeText(ContractType contractType)
    {
        return contractType switch
        {
            ContractType.NewSign => "新签",
            ContractType.Renewal => "续签",
            _ => "未知"
        };
    }
}
