using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;

namespace CRM.Application.Contracts;

public class ContractAppService : IContractAppService
{
    private readonly IRepository<Contract, int> _contractRepo;
    private readonly IRepository<Customer, int> _customerRepo;

    public ContractAppService(IRepository<Contract, int> contractRepo, IRepository<Customer, int> customerRepo)
    {
        _contractRepo = contractRepo;
        _customerRepo = customerRepo;
    }

    public async Task<List<ContractDto>> GetListAsync(string? keyword = null)
    {
        var contracts = await _contractRepo.GetListAsync();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var searchKey = keyword.Trim();
            contracts = contracts.Where(c =>
                c.ContractNo.Contains(searchKey) ||
                c.ContractName.Contains(searchKey) ||
                c.CustomerName.Contains(searchKey)).ToList();
        }

        return contracts.Select(MapToDto).ToList();
    }

    public async Task<ContractDto> GetAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) throw new BusinessException("合同不存在");

        return MapToDto(contract);
    }

    public async Task CreateAsync(CreateContractDto input)
    {
        var contract = new Contract(
            input.ContractNo ?? "",
            input.ContractName ?? "",
            input.CustomerId,
            input.CustomerName ?? "",
            input.ContactId,
            input.ContactName,
            input.TotalAmount,
            input.SignDate,
            input.StartDate,
            input.EndDate,
            input.PaymentFrequency,
            input.RegionalCompany,
            input.AffiliatedCompany,
            input.ServiceType,
            input.ContractType,
            input.Remark);

        foreach (var item in input.Items)
        {
            contract.AddItem(item.ProductName ?? "", item.Quantity, item.UnitPrice);
        }

        foreach (var plan in input.PaymentPlans)
        {
            contract.AddPaymentPlan(plan.PlanDate, plan.PlanAmount, plan.Description);
        }

        await _contractRepo.InsertAsync(contract);
    }

    public async Task UpdateAsync(UpdateContractDto input)
    {
        var contract = await _contractRepo.GetByIdAsync(input.Id);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.UpdateBasicInfo(
            input.ContractNo ?? "",
            input.ContractName ?? "",
            input.CustomerId,
            input.CustomerName ?? "",
            input.ContactId,
            input.ContactName,
            input.TotalAmount,
            input.SignDate,
            input.StartDate,
            input.EndDate,
            input.PaymentFrequency,
            input.RegionalCompany,
            input.AffiliatedCompany,
            input.ServiceType,
            input.ContractType,
            input.Remark);
        contract.ChangeStatus((ContractStatus)input.Status);

        await _contractRepo.UpdateAsync(contract);
    }

    public async Task DeleteAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) return;

        await _contractRepo.DeleteAsync(contract);
    }

    public async Task<List<ContactSelectDto>> GetContactsByCustomerIdAsync(int customerId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) return new List<ContactSelectDto>();

        return customer.Contacts.Select(c => new ContactSelectDto
        {
            ContactId = c.Id,
            ContactName = c.Name,
            Phone = c.Phone,
            Email = c.Email
        }).ToList();
    }

    private static ContractDto MapToDto(Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ContractNo = contract.ContractNo,
            ContractName = contract.ContractName,
            SignDate = contract.SignDate,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            TotalAmount = contract.TotalAmount,
            Status = (int)contract.Status,
            CustomerId = contract.CustomerId,
            CustomerName = contract.CustomerName,
            ContactId = contract.ContactId,
            ContactName = contract.ContactName,
            CreationTime = contract.CreationTime,
            Remark = contract.Remark,
            Items = contract.Items.Select(i => new ContractItemDto
            {
                Id = i.Id,
                ContractId = i.ContractId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal
            }).ToList(),
            PaymentPlans = contract.PaymentPlans.Select(p => new PaymentPlanDto
            {
                Id = p.Id,
                ContractId = p.ContractId,
                PlanDate = p.PlanDate,
                PlanAmount = p.PlanAmount,
                ActualAmount = p.ActualAmount,
                ActualDate = p.ActualDate,
                Status = (int)p.Status,
                Description = p.Description
            }).ToList()
        };
    }
}
