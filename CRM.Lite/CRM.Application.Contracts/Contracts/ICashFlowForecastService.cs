using CRM.Application.Contracts.Contracts.Dtos;

namespace CRM.Application.Contracts.Contracts;

/// <summary>
/// 现金流预测服务接口
/// </summary>
public interface ICashFlowForecastService
{
    /// <summary>
    /// 获取未来 N 个月的现金流预测数据
    /// </summary>
    /// <param name="months">预测月数（默认 12）</param>
    Task<List<CashFlowForecastDto>> GetForecastAsync(int months = 12);
}
