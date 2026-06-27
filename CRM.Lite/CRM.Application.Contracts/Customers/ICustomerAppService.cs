using CRM.Application.Contracts.Common.Dtos;
using CRM.Application.Contracts.Customers.Dtos;

namespace CRM.Application.Contracts.Customers;

public interface ICustomerAppService
{
    Task<PagedResultDto<CustomerDto>> GetPagedListAsync(CustomerQueryDto input);
    Task<CustomerDetailDto> GetDetailAsync(int id);
    Task<CustomerDto> GetByIdAsync(int id);
    Task CreateAsync(CreateCustomerDto input);
    Task UpdateAsync(UpdateCustomerDto input);
    Task DeleteAsync(int id);
}
