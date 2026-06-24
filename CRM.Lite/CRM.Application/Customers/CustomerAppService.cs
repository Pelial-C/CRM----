using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.ValueObjects;

namespace CRM.Application.Customers;

public class CustomerAppService : ICustomerAppService
{
    private readonly IRepository<Customer, int> _customerRepo;

    // 通过构造函数注入泛型仓储
    public CustomerAppService(IRepository<Customer, int> customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<List<CustomerDto>> GetListAsync(string? keyword = null)
    {
        var customers = await _customerRepo.GetListAsync();

        // 简单的内存过滤 (如果数据量大，需要在 IRepository 中扩展带 Expression 的查询方法)
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            // 1. 提取为一个明确非空的局部变量，打消编译器对 keyword 的疑虑
            string searchKey = keyword;

            customers = customers.Where(c =>
                // 2. 使用 ?. 防止 c.Name 或 c.CreditCode 本身为 null 时引发空指针
                (c.Name?.Contains(searchKey) == true) ||
                (c.CreditCode?.Contains(searchKey) == true)
            ).ToList();
        }

        // 手动映射 Entity 到 DTO (企业级项目通常用 AutoMapper，这里手写更直观)
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            CreditCode = c.CreditCode,
            Industry = c.Industry,
            Province = c.Address?.Province,
            City = c.Address?.City,
            District = c.Address?.District,
            DetailAddress = c.Address?.Detail,
            CreationTime = c.CreationTime
        }).ToList();
    }

    public async Task<CustomerDto> GetByIdAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) throw new BusinessException("客户不存在");

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            CreditCode = customer.CreditCode,
            Industry = customer.Industry,
            Province = customer.Address?.Province,
            City = customer.Address?.City,
            District = customer.Address?.District,
            DetailAddress = customer.Address?.Detail,
            CreationTime = customer.CreationTime
        };
    }

    public async Task CreateAsync(CreateCustomerDto input)
    {
        // 1. 组装值对象
        var address = new Address(input.Province ?? "", input.City ?? "", input.District ?? "", input.DetailAddress ?? "");

        // 2. 调用充血构造函数创建聚合根 (内部会自动进行基础校验)
        var customer = new Customer(input.Name ?? "", input.CreditCode ?? "", input.Industry ?? "", address);

        // 3. 持久化
        await _customerRepo.InsertAsync(customer);
    }

    public async Task UpdateAsync(UpdateCustomerDto input)
    {
        var customer = await _customerRepo.GetByIdAsync(input.Id);
        if (customer == null) throw new BusinessException("客户不存在");

        // 1. 组装值对象
        var address = new Address(input.Province ?? "", input.City ?? "", input.District ?? "", input.DetailAddress ?? "");

        // 2. 调用聚合根的充血行为方法更新状态
        customer.UpdateInfo(input.Name ?? "", input.CreditCode ?? "", input.Industry ?? "", address);

        // 3. 持久化
        await _customerRepo.UpdateAsync(customer);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return; // 幂等性设计：不存在就不报错

        await _customerRepo.DeleteAsync(customer);
    }
}
