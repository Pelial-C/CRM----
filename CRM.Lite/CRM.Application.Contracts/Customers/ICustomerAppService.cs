using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Application.Contracts.Customers.Dtos;

namespace CRM.Application.Contracts.Customers;

public interface ICustomerAppService
{
    Task<List<CustomerDto>> GetListAsync(string? keyword = null);
    Task<List<CustomerDto>> GetListAsync(CustomerQueryDto query);
    Task<List<CustomerSelectDto>> GetSelectListAsync();
    Task<CustomerDto> GetByIdAsync(int id);
    Task CreateAsync(CreateCustomerDto input);
    Task UpdateAsync(UpdateCustomerDto input);
    Task DeleteAsync(int id);
}
