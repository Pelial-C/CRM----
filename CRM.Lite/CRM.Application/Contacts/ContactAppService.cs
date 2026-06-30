using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Contacts;

public class ContactAppService : IContactAppService
{
    private readonly IRepository<Customer, int> _customerRepo;

    public ContactAppService(IRepository<Customer, int> customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<List<ContactDto>> GetListByCustomerIdAsync(int customerId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) throw new BusinessException("客户不存在");

        return customer.Contacts
            .OrderBy(c => c.Id)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ContactDto> GetAsync(int id)
    {
        var contact = await FindContactAsync(id);
        return MapToDto(contact.Contact);
    }

    public async Task<ContactDto> CreateAsync(int customerId, CreateContactDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) throw new BusinessException("客户不存在");
        if (customer.IsDeleted) throw new BusinessException("已删除客户不能新增联系人");

        var contact = customer.AddContact(input.Name ?? string.Empty, input.Phone, input.Title, input.Email, input.IsKeyDecisionMaker);
        await _customerRepo.UpdateAsync(customer);

        return MapToDto(contact);
    }

    public async Task UpdateAsync(UpdateContactDto input)
    {
        var result = await FindContactAsync(input.Id);
        if (result.Customer.IsDeleted) throw new BusinessException("已删除客户不能维护联系人");

        result.Contact.Update(input.Name ?? string.Empty, input.Phone, input.Title, input.Email, input.IsKeyDecisionMaker);
        await _customerRepo.UpdateAsync(result.Customer);
    }

    public async Task DeleteAsync(int id)
    {
        var result = await FindContactAsync(id);
        if (result.Customer.IsDeleted) throw new BusinessException("已删除客户不能维护联系人");

        result.Customer.RemoveContact(id);
        await _customerRepo.UpdateAsync(result.Customer);
    }

    private async Task<(Customer Customer, Contact Contact)> FindContactAsync(int id)
    {
        var customer = await _customerRepo.Query()
            .FirstOrDefaultAsync(c => c.Contacts.Any(contact => contact.Id == id));

        if (customer == null) throw new BusinessException("联系人不存在");

        var contact = customer.Contacts.First(c => c.Id == id);
        return (customer, contact);
    }

    private static ContactDto MapToDto(Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            CustomerId = contact.CustomerId,
            Name = contact.Name,
            Title = contact.Title,
            Phone = contact.Phone,
            Email = contact.Email,
            IsKeyDecisionMaker = contact.IsKeyDecisionMaker
        };
    }
}
