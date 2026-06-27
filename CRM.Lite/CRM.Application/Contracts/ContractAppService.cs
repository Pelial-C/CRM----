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

    public async Task<PagedResultDto<ContractDto>> GetListAsync(GetContractListDto input)
    {
        var contracts = await _contractRepo.GetListAsync();
        var query = contracts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            var searchKey = input.Keyword.Trim();
            query = query.Where(c =>
                c.ContractNo.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.ContractName.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                c.CustomerName.Contains(searchKey, StringComparison.OrdinalIgnoreCase));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(c => (int)c.Status == input.Status.Value);
        }

        if (input.CustomerId.HasValue)
        {
            query = query.Where(c => c.CustomerId == input.CustomerId.Value);
        }

        var ordered = query.OrderByDescending(c => c.CreationTime).ToList();
        var pageIndex = input.PageIndex < 1 ? 1 : input.PageIndex;
        var pageSize = input.PageSize < 1 ? 10 : input.PageSize;
        var totalCount = ordered.Count;

        var pageItems = ordered
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResultDto<ContractDto>
        {
            Items = pageItems,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<ContractDto> GetAsync(int id)
    {
        var contract = await GetContractOrThrowAsync(id);
        return MapToDto(contract);
    }

    public async Task CreateAsync(CreateContractDto input)
    {
        var (customerName, contactName) = await ResolveCustomerAndContactAsync(input.CustomerId, input.ContactId);

        var contract = new Contract(
            input.ContractNo ?? "",
            input.ContractName ?? "",
            input.CustomerId,
            customerName,
            input.ContactId,
            contactName,
            input.TotalAmount,
            input.SignDate,
            input.StartDate,
            input.EndDate,
            input.PaymentFrequency,
            input.RegionalCompany,
            input.AffiliatedCompany,
            input.ServiceType,
            input.ContractType,
            input.Remark,
            input.CabinetNo,
            input.WarningDays);

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
        var contract = await GetContractOrThrowAsync(input.Id);
        var (customerName, contactName) = await ResolveCustomerAndContactAsync(input.CustomerId, input.ContactId);

        contract.UpdateBasicInfo(
            input.ContractNo ?? "",
            input.ContractName ?? "",
            input.CustomerId,
            customerName,
            input.ContactId,
            contactName,
            input.TotalAmount,
            input.SignDate,
            input.StartDate,
            input.EndDate,
            input.PaymentFrequency,
            input.RegionalCompany,
            input.AffiliatedCompany,
            input.ServiceType,
            input.ContractType,
            input.Remark,
            input.CabinetNo,
            input.WarningDays);
        contract.ChangeStatus((ContractStatus)input.Status);

        var items = input.Items.Select(i => (i.ProductName ?? "", i.Quantity, i.UnitPrice));
        contract.ReplaceItems(items);

        await _contractRepo.UpdateAsync(contract);
    }

    public async Task CancelAsync(CancelContractDto input)
    {
        var contract = await GetContractOrThrowAsync(input.Id);
        contract.Cancel(input.Reason);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task GeneratePaymentPlansAsync(int contractId)
    {
        var contract = await GetContractOrThrowAsync(contractId);
        contract.GeneratePaymentPlans();
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task AddPaymentPlanAsync(AddPaymentPlanDto input)
    {
        var contract = await GetContractOrThrowAsync(input.ContractId);
        contract.AddPaymentPlan(input.PlanDate, input.PlanAmount, input.Description);
        await _contractRepo.UpdateAsync(contract);
    }

    public async Task RecordPaymentAsync(RecordPaymentDto input)
    {
        var contract = await GetContractOrThrowAsync(input.ContractId);
        contract.RecordPayment(input.PlanId, input.ActualAmount, input.ActualDate);
        await _contractRepo.UpdateAsync(contract);
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

    private async Task<Contract> GetContractOrThrowAsync(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null) throw new BusinessException("合同不存在");
        return contract;
    }

    private async Task<(string CustomerName, string? ContactName)> ResolveCustomerAndContactAsync(int customerId, int? contactId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null) throw new BusinessException("客户不存在");
        if (customer.IsDeleted) throw new BusinessException("客户已删除，不能关联合同");

        string? contactName = null;
        if (contactId.HasValue)
        {
            var contact = customer.Contacts.FirstOrDefault(c => c.Id == contactId.Value);
            if (contact == null) throw new BusinessException("联系人不属于该客户");
            contactName = contact.Name;
        }

        return (customer.Name, contactName);
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
            ServiceType = (int)contract.ServiceType,
            ContractType = (int)contract.ContractType,
            PaymentFrequency = (int)contract.PaymentFrequency,
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
