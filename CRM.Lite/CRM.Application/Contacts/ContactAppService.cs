using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Contacts;

public class ContactAppService : IContactAppService
{
    private readonly IRepository<Customer, int> _customerRepo;
    private readonly IRepository<Contract, int> _contractRepo;

    public ContactAppService(IRepository<Customer, int> customerRepo, IRepository<Contract, int> contractRepo)
    {
        _customerRepo = customerRepo;
        _contractRepo = contractRepo;
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
        EnsureContactUnique(customer, input.Name, input.Phone);

        var contact = customer.AddContact(input.Name ?? string.Empty, input.Phone, input.Title, input.Email, input.IsKeyDecisionMaker);
        await _customerRepo.UpdateAsync(customer);

        return MapToDto(contact);
    }

    public async Task UpdateAsync(UpdateContactDto input)
    {
        var result = await FindContactAsync(input.Id);
        if (result.Customer.IsDeleted) throw new BusinessException("已删除客户不能维护联系人");
        EnsureContactUnique(result.Customer, input.Name, input.Phone, input.Id);

        result.Contact.Update(input.Name ?? string.Empty, input.Phone, input.Title, input.Email, input.IsKeyDecisionMaker);
        await _customerRepo.UpdateAsync(result.Customer);
    }

    public async Task DeleteAsync(int id)
    {
        var result = await FindContactAsync(id);
        if (result.Customer.IsDeleted) throw new BusinessException("已删除客户不能维护联系人");
        var isUsedByContract = await _contractRepo.Query().AnyAsync(c => c.ContactId == id);
        if (isUsedByContract) throw new BusinessException("该联系人已被合同使用，不能删除");

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

    private static void EnsureContactUnique(Customer customer, string? name, string? phone, int? excludeId = null)
    {
        var normalizedName = (name ?? string.Empty).Trim();
        var normalizedPhone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        var exists = customer.Contacts.Any(c =>
            (!excludeId.HasValue || c.Id != excludeId.Value) &&
            c.Name == normalizedName &&
            (c.Phone ?? string.Empty) == (normalizedPhone ?? string.Empty));

        if (exists) throw new BusinessException("同一客户下联系人姓名和手机号不能重复");
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
