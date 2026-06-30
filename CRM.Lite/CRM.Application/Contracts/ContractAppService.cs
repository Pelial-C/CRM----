using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

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
        var result = await GetListAsync(new ContractListQueryDto { Keyword = keyword, PageSize = 1000 });
        return result.Items;
    }

    public async Task<PagedResultDto<ContractDto>> GetListAsync(ContractListQueryDto query)
    {
        var pageIndex = query.PageIndex <= 0 ? 1 : query.PageIndex;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 100);
        var contracts = _contractRepo.Query().IgnoreAutoIncludes();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var searchKey = query.Keyword.Trim();
            contracts = contracts.Where(c =>
                c.ContractNo.Contains(searchKey) ||
                c.ContractName.Contains(searchKey) ||
                c.CustomerName.Contains(searchKey));
        }

        if (query.Status.HasValue)
        {
            var status = (ContractStatus)query.Status.Value;
            contracts = contracts.Where(c => c.Status == status);
        }

        if (query.CustomerId.HasValue && query.CustomerId.Value > 0)
        {
            contracts = contracts.Where(c => c.CustomerId == query.CustomerId.Value);
        }

        var totalCount = await contracts.CountAsync();
        var items = await contracts
            .OrderByDescending(c => c.CreationTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ContractDto
            {
                Id = c.Id,
                ContractNo = c.ContractNo,
                ContractName = c.ContractName,
                CabinetNo = c.CabinetNo,
                SignDate = c.SignDate,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                TotalAmount = c.TotalAmount,
                Status = (int)c.Status,
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                ContactId = c.ContactId,
                ContactName = c.ContactName,
                PaymentFrequency = c.PaymentFrequency,
                ServiceType = c.ServiceType,
                ContractType = c.ContractType,
                WarningDays = c.WarningDays,
                RegionalCompany = c.RegionalCompany,
                AffiliatedCompany = c.AffiliatedCompany,
                CreationTime = c.CreationTime,
                Remark = c.Remark
            })
            .ToListAsync();

        return new PagedResultDto<ContractDto>
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<ContractDto> GetAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.RefreshOverduePaymentPlans(DateTime.Today);
        await _contractRepo.UpdateAsync(contract);

        return MapToDto(contract);
    }

    public async Task CreateAsync(CreateContractDto input)
    {
        if (!await CheckContractNoUniqueAsync(input.ContractNo ?? string.Empty))
        {
            throw new BusinessException("合同编号已存在");
        }

        var customer = await GetValidCustomerAsync(input.CustomerId);
        var contact = GetValidContact(customer, input.ContactId);

        var contract = new Contract(
            input.ContractNo ?? string.Empty,
            input.ContractName ?? string.Empty,
            customer.Id,
            customer.Name,
            contact?.Id,
            contact?.Name,
            input.TotalAmount,
            input.SignDate,
            input.StartDate,
            input.EndDate,
            input.PaymentFrequency,
            input.RegionalCompany,
            input.AffiliatedCompany,
            input.ServiceType,
            input.ContractType,
            input.CabinetNo,
            input.WarningDays,
            input.Remark);

        var items = input.Items
            .Where(i => !string.IsNullOrWhiteSpace(i.ProductName))
            .Select(i => (i.ProductName ?? string.Empty, i.Quantity, i.UnitPrice))
            .ToList();
        contract.ReplaceItems(items);

        foreach (var plan in input.PaymentPlans.Where(p => p.PlanAmount > 0))
        {
            contract.AddPaymentPlan(plan.PlanDate, plan.PlanAmount, plan.Description);
        }

        await _contractRepo.InsertAsync(contract);
    }

    public async Task UpdateAsync(UpdateContractDto input)
    {
        var contract = await _contractRepo.GetByIdAsync(input.Id);
        if (contract == null) throw new BusinessException("合同不存在");

        if (!await CheckContractNoUniqueAsync(input.ContractNo ?? string.Empty, input.Id))
        {
            throw new BusinessException("合同编号已存在");
        }

        var customer = await GetValidCustomerAsync(input.CustomerId);
        var contact = GetValidContact(customer, input.ContactId);

        contract.UpdateBasicInfo(
            input.ContractNo ?? string.Empty,
            input.ContractName ?? string.Empty,
            customer.Id,
            customer.Name,
            contact?.Id,
            contact?.Name,
            input.TotalAmount,
            input.SignDate,
            input.StartDate,
            input.EndDate,
            input.PaymentFrequency,
            input.RegionalCompany,
            input.AffiliatedCompany,
            input.ServiceType,
            input.ContractType,
            input.CabinetNo,
            input.WarningDays,
            input.Remark);

        contract.ReplaceItems(input.Items
            .Where(i => !string.IsNullOrWhiteSpace(i.ProductName))
            .Select(i => (i.ProductName ?? string.Empty, i.Quantity, i.UnitPrice)));

        await _contractRepo.UpdateAsync(contract);
    }

    public async Task DeleteAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) return;

        await _contractRepo.DeleteAsync(contract);
    }

    public async Task StartAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.StartExecution();
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task CancelAsync(int id, string? reason = null)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.Cancel(reason);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task TerminateAsync(int id, string? reason = null)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.Terminate(reason);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task GeneratePaymentPlansAsync(int contractId)
    {
        var contract = await _contractRepo.GetByIdAsync(contractId);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.GeneratePaymentPlans();
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task AddPaymentPlanAsync(AddPaymentPlanDto input)
    {
        var contract = await _contractRepo.GetByIdAsync(input.ContractId);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.AddPaymentPlan(input.PlanDate, input.PlanAmount, input.Description);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task RecordPaymentAsync(RecordPaymentDto input)
    {
        var contract = await _contractRepo.GetByIdAsync(input.ContractId);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.RecordPayment(input.PlanId, input.ActualAmount, input.ActualDate);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task RefreshOverduePaymentPlansAsync(int contractId)
    {
        var contract = await _contractRepo.GetByIdAsync(contractId);
        if (contract == null) throw new BusinessException("合同不存在");

        contract.RefreshOverduePaymentPlans(DateTime.Today);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task<bool> CheckContractNoUniqueAsync(string contractNo, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(contractNo)) return false;

        var normalized = contractNo.Trim();
        return !await _contractRepo.Query()
            .AnyAsync(c => c.ContractNo == normalized && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<List<ContactSelectDto>> GetContactsByCustomerIdAsync(int customerId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null || customer.IsDeleted) return new List<ContactSelectDto>();

        return customer.Contacts
            .OrderBy(c => c.Id)
            .Select(c => new ContactSelectDto
            {
                ContactId = c.Id,
                ContactName = c.Name,
                Phone = c.Phone,
                Email = c.Email
            })
            .ToList();
    }

    private async Task<Customer> GetValidCustomerAsync(int customerId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null || customer.IsDeleted) throw new BusinessException("客户不存在或已删除");

        return customer;
    }

    private static Contact? GetValidContact(Customer customer, int? contactId)
    {
        if (!contactId.HasValue) return null;

        var contact = customer.Contacts.FirstOrDefault(c => c.Id == contactId.Value);
        if (contact == null) throw new BusinessException("联系人不属于所选客户");

        return contact;
    }

    private static ContractDto MapToDto(Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ContractNo = contract.ContractNo,
            ContractName = contract.ContractName,
            CabinetNo = contract.CabinetNo,
            SignDate = contract.SignDate,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            TotalAmount = contract.TotalAmount,
            Status = (int)contract.Status,
            CustomerId = contract.CustomerId,
            CustomerName = contract.CustomerName,
            ContactId = contract.ContactId,
            ContactName = contract.ContactName,
            PaymentFrequency = contract.PaymentFrequency,
            ServiceType = contract.ServiceType,
            ContractType = contract.ContractType,
            WarningDays = contract.WarningDays,
            RegionalCompany = contract.RegionalCompany,
            AffiliatedCompany = contract.AffiliatedCompany,
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
