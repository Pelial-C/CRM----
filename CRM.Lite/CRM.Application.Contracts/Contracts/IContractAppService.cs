using CRM.Application.Contracts.Contracts.Dtos;

namespace CRM.Application.Contracts.Contracts;

public interface IContractAppService
{
    Task<List<ContractDto>> GetListAsync(string? keyword = null);
    Task<PagedResultDto<ContractDto>> GetListAsync(ContractListQueryDto query);
    Task<ContractDto> GetAsync(int id);
    Task CreateAsync(CreateContractDto input);
    Task UpdateAsync(UpdateContractDto input);
    Task DeleteAsync(int id);
    Task StartAsync(int id);
    Task CancelAsync(int id, string? reason = null);
    Task TerminateAsync(int id, string? reason = null);
    Task GeneratePaymentPlansAsync(int contractId);
    Task AddPaymentPlanAsync(AddPaymentPlanDto input);
    Task RecordPaymentAsync(RecordPaymentDto input);
    Task RefreshOverduePaymentPlansAsync(int contractId);
    Task<bool> CheckContractNoUniqueAsync(string contractNo, int? excludeId = null);
    Task<List<ContactSelectDto>> GetContactsByCustomerIdAsync(int customerId);
}
