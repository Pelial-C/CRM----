using CRM.Application.Contracts.Contacts.Dtos;

namespace CRM.Application.Contracts.Contacts;

public interface IContactAppService
{
    Task<List<ContactDto>> GetListAsync(ContactQueryDto input);
    Task<ContactDto> GetByIdAsync(int customerId, int contactId);
    Task CreateAsync(CreateContactDto input);
    Task UpdateAsync(int customerId, UpdateContactDto input);
    Task DeleteAsync(int customerId, int contactId);
}
