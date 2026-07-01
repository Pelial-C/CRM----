namespace CRM.Domain.Shared.Enums;

/// <summary>
/// 客户等级枚举
/// </summary>
public enum CustomerLevel
{
    /// <summary>战略客户 — 总分 >= 80</summary>
    Strategic = 0,

    /// <summary>重点客户 — 总分 >= 60</summary>
    Important = 1,

    /// <summary>普通客户 — 总分 >= 40</summary>
    Normal = 2,

    /// <summary>风险客户 — 总分 < 40</summary>
    Risk = 3
}
