using CRM.Application.Contracts.Common.Dtos;
using CRM.Application.Contracts.Contracts.Dtos;

namespace CRM.Application.Contracts.Contracts;

public interface IContractAppService
{
    // 查询
    Task<PagedResultDto<ContractDto>> GetListAsync(GetContractListDto input);
    Task<ContractDto> GetAsync(int id);

    // 增删改
    Task CreateAsync(CreateContractDto input);
    Task UpdateAsync(UpdateContractDto input);

    // 合同状态管理
    Task CancelAsync(CancelContractDto input);

    // 回款计划管理
    Task GeneratePaymentPlansAsync(int contractId);
    Task AddPaymentPlanAsync(AddPaymentPlanDto input);
    Task RecordPaymentAsync(RecordPaymentDto input);

    // 联动查询
    Task<List<ContactSelectDto>> GetContactsByCustomerIdAsync(int customerId);
}
