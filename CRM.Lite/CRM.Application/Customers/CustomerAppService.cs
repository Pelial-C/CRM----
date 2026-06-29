using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Customers;

public class CustomerAppService : ICustomerAppService
{
    private readonly IRepository<Customer, int> _customerRepo;

    public CustomerAppService(IRepository<Customer, int> customerRepo)
    {
        _customerRepo = customerRepo;
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
        var address = new Address(input.Province ?? "", input.City ?? "", input.District ?? "", input.DetailAddress ?? "");
        var customer = new Customer(input.Name ?? "", input.CreditCode ?? "", input.Industry ?? "", address);
        await _customerRepo.InsertAsync(customer);
    }

    public async Task UpdateAsync(UpdateCustomerDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(input.Id);
        if (customer == null) throw new BusinessException("客户不存在");

        var address = new Address(input.Province ?? "", input.City ?? "", input.District ?? "", input.DetailAddress ?? "");
        customer.UpdateInfo(input.Name ?? "", input.CreditCode ?? "", input.Industry ?? "", address);
        await _customerRepo.UpdateAsync(customer);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return;

        customer.MarkAsDeleted();
        await _customerRepo.UpdateAsync(customer);
    }

    private IQueryable<Customer> BuildPredicate(CustomerQueryDto query)
    {
        var customers = _customerRepo.Query().IgnoreAutoIncludes();

        if (!query.IncludeDeleted)
        {
            customers = customers.Where(c => !c.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var searchKey = query.Keyword.Trim();
            customers = customers.Where(c =>
                c.Name.Contains(searchKey) ||
                (c.CreditCode != null && c.CreditCode.Contains(searchKey)));
        }

        return customers;
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreditCode = customer.CreditCode,
            Industry = customer.Industry,
            IsDeleted = customer.IsDeleted,
            Province = customer.Address.Province,
            City = customer.Address.City,
            District = customer.Address.District,
            DetailAddress = customer.Address.Detail,
            CreationTime = customer.CreationTime
        };
    }
}
