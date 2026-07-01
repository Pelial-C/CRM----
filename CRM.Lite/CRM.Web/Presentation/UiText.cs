using CRM.Domain.Contracts;
using CRM.Domain.Shared.Enums;

namespace CRM.Web.Presentation;

public static class UiText
{
    public static string GetContractStatusName(int status)
    {
        return Enum.IsDefined(typeof(ContractStatus), status)
            ? GetContractStatusName((ContractStatus)status)
            : status.ToString();
    }

    public static string GetContractStatusName(ContractStatus status)
    {
        return status switch
        {
            ContractStatus.Draft => "草稿",
            ContractStatus.Active => "履约中",
            ContractStatus.Completed => "已完成",
            ContractStatus.Cancelled => "已作废",
            ContractStatus.Terminated => "已终止",
            _ => status.ToString()
        };
    }

    public static string GetPaymentPlanStatusName(int status)
    {
        return Enum.IsDefined(typeof(PaymentPlanStatus), status)
            ? ((PaymentPlanStatus)status) switch
            {
                PaymentPlanStatus.Pending => "待回款",
                PaymentPlanStatus.PartialPaid => "部分回款",
                PaymentPlanStatus.Paid => "已回款",
                PaymentPlanStatus.Overdue => "已逾期",
                _ => status.ToString()
            }
            : status.ToString();
    }

    public static string GetServiceTypeName(int value)
    {
        return Enum.IsDefined(typeof(ServiceType), value)
            ? GetServiceTypeName((ServiceType)value)
            : value.ToString();
    }

    public static string GetServiceTypeName(ServiceType value)
    {
        return value switch
        {
            ServiceType.Software => "软件销售",
            ServiceType.Implementation => "实施服务",
            ServiceType.Maintenance => "维保服务",
            ServiceType.Consulting => "咨询服务",
            _ => value.ToString()
        };
    }

    public static string GetContractTypeName(int value)
    {
        return Enum.IsDefined(typeof(ContractType), value)
            ? GetContractTypeName((ContractType)value)
            : value.ToString();
    }

    public static string GetContractTypeName(ContractType value)
    {
        return value switch
        {
            ContractType.NewSign => "新签",
            ContractType.Renewal => "续签",
            _ => value.ToString()
        };
    }

    public static string GetPaymentFrequencyName(int value)
    {
        return Enum.IsDefined(typeof(PaymentFrequency), value)
            ? GetPaymentFrequencyName((PaymentFrequency)value)
            : value.ToString();
    }

    public static string GetPaymentFrequencyName(PaymentFrequency value)
    {
        return value switch
        {
            PaymentFrequency.Monthly => "按月",
            PaymentFrequency.Quarterly => "按季度",
            PaymentFrequency.HalfYearly => "按半年",
            PaymentFrequency.Yearly => "按年",
            _ => value.ToString()
        };
    }

    public static string YesNo(bool value)
    {
        return value ? "是" : "否";
    }
}
