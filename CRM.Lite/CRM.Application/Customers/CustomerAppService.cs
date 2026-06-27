using CRM.Application.Contracts.Common.Dtos;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;
using System.Linq.Expressions;

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

    public async Task<PagedResultDto<CustomerDto>> GetPagedListAsync(CustomerQueryDto input)
    {
        var predicate = BuildPredicate(input);

        var (items, totalCount) = await _customerRepo.GetPagedAsync(predicate, input.PageIndex, input.PageSize);

        var dtos = items.Select(MapToDto).ToList();

        return new PagedResultDto<CustomerDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = input.PageIndex,
            PageSize = input.PageSize
        };
    }

    public async Task<CustomerDetailDto> GetDetailAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) throw new BusinessException("客户不存在");

        var contracts = await _contractRepo.GetListAsync(c => c.CustomerId == id);

        return new CustomerDetailDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreditCode = customer.CreditCode,
            Industry = customer.Industry,
            Province = customer.Address?.Province,
            City = customer.Address?.City,
            District = customer.Address?.District,
            DetailAddress = customer.Address?.Detail,
            Remark = customer.Remark,
            IsDeleted = customer.IsDeleted,
            CreationTime = customer.CreationTime,
            Contacts = customer.Contacts.Select(MapContactToDto).ToList(),
            Contracts = contracts.Select(MapContractToSummaryDto).ToList()
        };
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) throw new BusinessException("客户不存在");

        return MapToDto(customer);
    }

    public async Task CreateAsync(CreateCustomerDto input)
    {
        await CheckNameUniqueAsync(input.Name!, null);

        var address = new Address(input.Province, input.City, input.District, input.DetailAddress);
        var customer = new Customer(input.Name!, input.CreditCode, input.Industry, address, input.Remark);

        await _customerRepo.InsertAsync(customer);
    }

    public async Task UpdateAsync(UpdateCustomerDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(input.Id);
        if (customer == null) throw new BusinessException("客户不存在");

        await CheckNameUniqueAsync(input.Name!, input.Id);

        var address = new Address(input.Province, input.City, input.District, input.DetailAddress);
        customer.UpdateInfo(input.Name!, input.CreditCode, input.Industry, address, input.Remark);

        await _customerRepo.UpdateAsync(customer);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return;

        var hasContracts = await _contractRepo.AnyAsync(c => c.CustomerId == id);

        if (customer.CanDelete(hasContracts))
        {
            await _customerRepo.DeleteAsync(customer);
        }
        else
        {
            customer.MarkAsDeleted();
            await _customerRepo.UpdateAsync(customer);
        }
    }

    private async Task CheckNameUniqueAsync(string name, int? excludeId)
    {
        var trimmed = name.Trim();
        var exists = await _customerRepo.AnyAsync(c => c.Name == trimmed && (!excludeId.HasValue || c.Id != excludeId.Value));
        if (exists) throw new BusinessException("企业名称已存在");
    }

    private static Expression<Func<Customer, bool>> BuildPredicate(CustomerQueryDto input)
    {
        return c =>
            (string.IsNullOrWhiteSpace(input.Name) || c.Name.Contains(input.Name.Trim())) &&
            (string.IsNullOrWhiteSpace(input.Industry) || c.Industry == input.Industry.Trim()) &&
            (!input.StartTime.HasValue || c.CreationTime >= input.StartTime.Value) &&
            (!input.EndTime.HasValue || c.CreationTime <= input.EndTime.Value);
    }

    private static CustomerDto MapToDto(Customer c)
    {
        return new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            CreditCode = c.CreditCode,
            Industry = c.Industry,
            Province = c.Address?.Province,
            City = c.Address?.City,
            District = c.Address?.District,
            DetailAddress = c.Address?.Detail,
            Remark = c.Remark,
            IsDeleted = c.IsDeleted,
            CreationTime = c.CreationTime
        };
    }

    private static ContactDto MapContactToDto(Contact c)
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

    private static ContractSummaryDto MapContractToSummaryDto(Contract c)
    {
        return new ContractSummaryDto
        {
            Id = c.Id,
            ContractNo = c.ContractNo,
            ContractName = c.ContractName,
            TotalAmount = c.TotalAmount,
            Status = (int)c.Status,
            SignDate = c.SignDate
        };
    }
}
