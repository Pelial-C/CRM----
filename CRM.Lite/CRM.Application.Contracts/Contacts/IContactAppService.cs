using CRM.Application.Contracts.Contacts.Dtos;

namespace CRM.Application.Contracts.Contacts;

public interface IContactAppService
{
    Task<List<ContactDto>> GetListByCustomerIdAsync(int customerId);
    Task<ContactDto> GetAsync(int id);
    Task<ContactDto> CreateAsync(int customerId, CreateContactDto input);
    Task UpdateAsync(UpdateContactDto input);
    Task DeleteAsync(int id);
}
