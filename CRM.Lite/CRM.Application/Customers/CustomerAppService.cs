using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Customers;

public class CustomerAppService : ICustomerAppService
{
    private readonly IRepository<Customer, int> _customerRepo;
    private readonly IRepository<Contract, int> _contractRepo;

    public CustomerAppService(IRepository<Customer, int> customerRepo, IRepository<Contract, int> contractRepo)
    {
        _customerRepo = customerRepo;
        _contractRepo = contractRepo;
    }

    public Task<List<CustomerDto>> GetListAsync(string? keyword = null)
    {
        return GetListAsync(new CustomerQueryDto { Keyword = keyword });
    }

    public async Task<List<CustomerDto>> GetListAsync(CustomerQueryDto query)
    {
        var customers = BuildPredicate(query);
        return await customers
            .OrderByDescending(c => c.CreationTime)
            .ThenByDescending(c => c.Id)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<List<CustomerSelectDto>> GetSelectListAsync()
    {
        return await _customerRepo.Query()
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreationTime)
            .ThenByDescending(c => c.Id)
            .Select(c => new CustomerSelectDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) throw new BusinessException("客户不存在");

        return MapToDto(customer);
    }

    public async Task CreateAsync(CreateCustomerDto input)
    {
        await CheckCustomerNameUniqueAsync(input.Name ?? string.Empty);
        await CheckCreditCodeUniqueAsync(input.CreditCode);

        var address = new Address(input.Province ?? string.Empty, input.City ?? string.Empty, input.District ?? string.Empty, input.DetailAddress ?? string.Empty);
        var customer = new Customer(input.Name ?? string.Empty, input.CreditCode, input.Industry, address, input.Remark);
        customer.SetOwner(input.OwnerUserId);
        await _customerRepo.InsertAsync(customer);
    }

    public async Task UpdateAsync(UpdateCustomerDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(input.Id);
        if (customer == null) throw new BusinessException("客户不存在");

        await CheckCustomerNameUniqueAsync(input.Name ?? string.Empty, input.Id);
        await CheckCreditCodeUniqueAsync(input.CreditCode, input.Id);

        var address = new Address(input.Province ?? string.Empty, input.City ?? string.Empty, input.District ?? string.Empty, input.DetailAddress ?? string.Empty);
        customer.UpdateInfo(input.Name ?? string.Empty, input.CreditCode, input.Industry, address, input.Remark);
        customer.SetOwner(input.OwnerUserId);
        await _customerRepo.UpdateAsync(customer);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return;

        var hasContracts = await _contractRepo.Query().AnyAsync(c => c.CustomerId == id);
        if (hasContracts) throw new BusinessException("该客户存在关联合同，不能直接删除");

        customer.MarkAsDeleted();
        await _customerRepo.UpdateAsync(customer);
    }

    private IQueryable<Customer> BuildPredicate(CustomerQueryDto query)
    {
        var customers = _customerRepo.Query();

        if (!query.IncludeDeleted)
        {
            customers = customers.Where(c => !c.IsDeleted);
        }

        if (query.OwnerUserId.HasValue)
        {
            customers = customers.Where(c => c.OwnerUserId == query.OwnerUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var searchKey = query.Keyword.Trim();
            customers = customers.Where(c =>
                c.Name.Contains(searchKey) ||
                (c.CreditCode != null && c.CreditCode.Contains(searchKey)));
        }

        if (!string.IsNullOrWhiteSpace(query.Industry))
        {
            var industry = query.Industry.Trim();
            customers = customers.Where(c => c.Industry != null && c.Industry.Contains(industry));
        }

        return customers;
    }

    private async Task CheckCustomerNameUniqueAsync(string name, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        var normalized = name.Trim();
        var exists = await _customerRepo.Query()
            .AnyAsync(c => c.Name == normalized && (!excludeId.HasValue || c.Id != excludeId.Value));

        if (exists) throw new BusinessException("客户名称已存在");
    }

    private async Task CheckCreditCodeUniqueAsync(string? creditCode, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(creditCode)) return;

        var normalized = creditCode.Trim();
        var exists = await _customerRepo.Query()
            .AnyAsync(c => c.CreditCode == normalized && (!excludeId.HasValue || c.Id != excludeId.Value));

        if (exists) throw new BusinessException("统一社会信用代码已存在");
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreditCode = customer.CreditCode,
            Industry = customer.Industry,
            Remark = customer.Remark,
            IsDeleted = customer.IsDeleted,
            OwnerUserId = customer.OwnerUserId,
            Province = customer.Address.Province,
            City = customer.Address.City,
            District = customer.Address.District,
            DetailAddress = customer.Address.Detail,
            CreationTime = customer.CreationTime,
            Contacts = customer.Contacts.Select(c => new CustomerContactDto
            {
                Id = c.Id,
                CustomerId = c.CustomerId,
                Name = c.Name,
                Title = c.Title,
                Phone = c.Phone,
                Email = c.Email,
                IsKeyDecisionMaker = c.IsKeyDecisionMaker
            }).ToList()
        };
    }
}
