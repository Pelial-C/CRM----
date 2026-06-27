using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;

namespace CRM.Application.Contacts;

public class ContactAppService : IContactAppService
{
    private readonly IRepository<Customer, int> _customerRepo;

    public ContactAppService(IRepository<Customer, int> customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<List<ContactDto>> GetListAsync(ContactQueryDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(input.CustomerId);
        if (customer == null) throw new BusinessException("客户不存在");

        var contacts = customer.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            var name = input.Name.Trim();
            contacts = contacts.Where(c => c.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(input.Title))
        {
            var title = input.Title.Trim();
            contacts = contacts.Where(c => c.Title != null && c.Title.Contains(title));
        }

        return contacts.Select(MapToDto).ToList();
    }

    public async Task<ContactDto> GetByIdAsync(int customerId, int contactId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) throw new BusinessException("客户不存在");

        var contact = customer.Contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null) throw new BusinessException("联系人不存在");

        return MapToDto(contact);
    }

    public async Task CreateAsync(CreateContactDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(input.CustomerId);
        if (customer == null) throw new BusinessException("客户不存在");

        customer.AddContact(input.Name!, input.Phone, input.Title, input.Email, input.IsKeyDecisionMaker);

        await _customerRepo.UpdateAsync(customer);
    }

    public async Task UpdateAsync(int customerId, UpdateContactDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) throw new BusinessException("客户不存在");

        customer.UpdateContact(input.Id, input.Name!, input.Phone, input.Title, input.Email, input.IsKeyDecisionMaker);

        await _customerRepo.UpdateAsync(customer);
    }

    public async Task DeleteAsync(int customerId, int contactId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) return;

        customer.RemoveContact(contactId);

        await _customerRepo.UpdateAsync(customer);
    }

    private static ContactDto MapToDto(Contact c)
    {
        return new ContactDto
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            Name = c.Name,
            Title = c.Title,
            Phone = c.Phone,
            Email = c.Email,
            IsKeyDecisionMaker = c.IsKeyDecisionMaker
        };
    }
}
