using CRM.Application.Contracts.Contracts.Dtos;

namespace CRM.Application.Contracts.Contracts;

public interface IContractAppService
{
    Task<List<ContractDto>> GetListAsync(string? keyword = null);
    Task<ContractDto> GetAsync(int id);
    Task CreateAsync(CreateContractDto input);
    Task UpdateAsync(UpdateContractDto input);
    Task DeleteAsync(int id);
    Task<List<ContactSelectDto>> GetContactsByCustomerIdAsync(int customerId);
}
