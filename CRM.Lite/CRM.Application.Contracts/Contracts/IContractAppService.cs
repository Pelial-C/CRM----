using CRM.Application.Contracts.Contracts.Dtos;

namespace CRM.Application.Contracts.Contracts;

public interface IContractAppService
{
    Task<PagedResultDto<ContractDto>> GetListAsync(GetContractListDto input);
    Task<ContractDto> GetAsync(int id);
    Task CreateAsync(CreateContractDto input);
    Task UpdateAsync(UpdateContractDto input);
    Task CancelAsync(CancelContractDto input);
    Task GeneratePaymentPlansAsync(int contractId);
    Task AddPaymentPlanAsync(AddPaymentPlanDto input);
    Task RecordPaymentAsync(RecordPaymentDto input);
    Task<List<ContactSelectDto>> GetContactsByCustomerIdAsync(int customerId);
}
