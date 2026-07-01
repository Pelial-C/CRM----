using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.Contracts.Events;

namespace CRM.Domain.Contracts;

public class Contract : AggregateRoot<int>
{
    public string ContractNo { get; private set; } = string.Empty;
    public string ContractName { get; private set; } = string.Empty;
    public string? CabinetNo { get; private set; }
    public DateTime SignDate { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public ContractStatus Status { get; private set; }
    public string? Remark { get; private set; }

    public int CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string PartyAName => CustomerName;
    public int? OwnerUserId { get; private set; }
    public int? ContactId { get; private set; }
    public string? ContactName { get; private set; }

    public string? RegionalCompany { get; private set; }
    public string? AffiliatedCompany { get; private set; }
    public ServiceType ServiceType { get; private set; }
    public ContractType ContractType { get; private set; }
    public int WarningDays { get; private set; }
    public PaymentFrequency PaymentFrequency { get; private set; }

    private readonly List<ContractItem> _items = new();
    public IReadOnlyCollection<ContractItem> Items => _items.AsReadOnly();

    private readonly List<PaymentPlan> _paymentPlans = new();
    public IReadOnlyCollection<PaymentPlan> PaymentPlans => _paymentPlans.AsReadOnly();

    protected Contract() { }

    public Contract(
        string contractNo,
        string contractName,
        int customerId,
        string customerName,
        int? contactId,
        string? contactName,
        decimal totalAmount,
        DateTime signDate,
        DateTime startDate,
        DateTime endDate,
        PaymentFrequency frequency,
        string? regionalCompany = null,
        string? affiliatedCompany = null,
        ServiceType serviceType = ServiceType.Software,
        ContractType contractType = ContractType.NewSign,
        string? cabinetNo = null,
        int warningDays = 30,
        string? remark = null)
    {
        Status = ContractStatus.Draft;
        UpdateBasicInfo(
            contractNo,
            contractName,
            customerId,
            customerName,
            contactId,
            contactName,
            totalAmount,
            signDate,
            startDate,
            endDate,
            frequency,
            regionalCompany,
            affiliatedCompany,
            serviceType,
            contractType,
            cabinetNo,
            warningDays,
            remark);
        AddDomainEvent(new ContractCreatedEvent(Id, ContractNo, CustomerId, TotalAmount, DateTime.Now));
    }

    public void UpdateBasicInfo(
        string contractNo,
        string contractName,
        int customerId,
        string customerName,
        int? contactId,
        string? contactName,
        decimal totalAmount,
        DateTime signDate,
        DateTime startDate,
        DateTime endDate,
        PaymentFrequency frequency,
        string? regionalCompany = null,
        string? affiliatedCompany = null,
        ServiceType serviceType = ServiceType.Software,
        ContractType contractType = ContractType.NewSign,
        string? cabinetNo = null,
        int warningDays = 30,
        string? remark = null)
    {
        if (!CanEdit()) throw new BusinessException("当前合同状态不允许编辑基础信息");
        if (string.IsNullOrWhiteSpace(contractNo)) throw new BusinessException("合同编号不能为空");
        if (string.IsNullOrWhiteSpace(contractName)) throw new BusinessException("合同名称不能为空");
        if (customerId <= 0) throw new BusinessException("合同必须关联客户");
        if (string.IsNullOrWhiteSpace(customerName)) throw new BusinessException("客户名称不能为空");
        if (contactId.HasValue && contactId.Value <= 0) throw new BusinessException("联系人Id必须有效");
        if (totalAmount <= 0) throw new BusinessException("合同金额必须大于0");
        if (endDate < startDate) throw new BusinessException("合同结束日期不能早于开始日期");
        if (warningDays < 0 || warningDays > 365) throw new BusinessException("预警天数必须在0到365之间");

        var plannedAmount = _paymentPlans.Sum(p => p.PlanAmount);
        if (plannedAmount > totalAmount) throw new BusinessException("回款计划累计金额不能超过合同总金额");

        ContractNo = contractNo.Trim();
        ContractName = contractName.Trim();
        CustomerId = customerId;
        CustomerName = customerName.Trim();
        ContactId = contactId;
        ContactName = string.IsNullOrWhiteSpace(contactName) ? null : contactName.Trim();
        TotalAmount = totalAmount;
        SignDate = signDate;
        StartDate = startDate;
        EndDate = endDate;
        PaymentFrequency = frequency;
        RegionalCompany = string.IsNullOrWhiteSpace(regionalCompany) ? null : regionalCompany.Trim();
        AffiliatedCompany = string.IsNullOrWhiteSpace(affiliatedCompany) ? null : affiliatedCompany.Trim();
        ServiceType = serviceType;
        ContractType = contractType;
        CabinetNo = string.IsNullOrWhiteSpace(cabinetNo) ? null : cabinetNo.Trim();
        WarningDays = warningDays;
        Remark = string.IsNullOrWhiteSpace(remark) ? null : remark.Trim();
    }

    public ContractItem AddItem(string productName, int quantity, decimal unitPrice)
    {
        if (!CanEdit()) throw new BusinessException("当前合同状态不允许维护合同明细");

        var item = new ContractItem(productName, quantity, unitPrice);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(int itemId)
    {
        if (!CanEdit()) throw new BusinessException("当前合同状态不允许维护合同明细");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new BusinessException("合同明细不存在");
        if (_items.Count == 1) throw new BusinessException("合同明细至少保留一项");

        _items.Remove(item);
    }

    public void ReplaceItems(IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        if (!CanEdit()) throw new BusinessException("当前合同状态不允许维护合同明细");

        var materializedItems = items.ToList();
        if (materializedItems.Count == 0) throw new BusinessException("合同明细至少包含一项");

        var newItems = materializedItems
            .Select(item => new ContractItem(item.ProductName, item.Quantity, item.UnitPrice))
            .ToList();
        var itemTotal = newItems.Sum(i => i.Subtotal);
        if (itemTotal != TotalAmount)
        {
            throw new BusinessException("合同明细金额总和必须等于合同总金额");
        }

        _items.Clear();
        foreach (var item in newItems)
        {
            _items.Add(item);
        }
    }

    public PaymentPlan AddPaymentPlan(DateTime planDate, decimal planAmount, string? description = null)
    {
        if (Status is not (ContractStatus.Draft or ContractStatus.Executing))
        {
            throw new BusinessException("当前合同状态不允许新增回款计划");
        }

        EnsurePaymentPlanTotal(planAmount);

        var paymentPlan = new PaymentPlan(planDate, planAmount, description);
        _paymentPlans.Add(paymentPlan);
        return paymentPlan;
    }

    public void GeneratePaymentPlans()
    {
        if (Status is not (ContractStatus.Draft or ContractStatus.Executing))
        {
            throw new BusinessException("当前合同状态不允许生成回款计划");
        }

        if (_paymentPlans.Any()) throw new BusinessException("回款计划已生成，请勿重复操作");

        var intervalMonths = (int)PaymentFrequency;
        if (intervalMonths <= 0) throw new BusinessException("回款频率无效");

        var planDates = new List<DateTime>();
        var currentDate = StartDate;
        while (currentDate < EndDate)
        {
            currentDate = currentDate.AddMonths(intervalMonths);
            if (currentDate > EndDate) currentDate = EndDate;
            planDates.Add(currentDate);
        }

        if (planDates.Count == 0)
        {
            planDates.Add(EndDate);
        }

        var averageAmount = Math.Round(TotalAmount / planDates.Count, 2);
        var remainingAmount = TotalAmount;

        for (var i = 0; i < planDates.Count; i++)
        {
            var currentAmount = i == planDates.Count - 1 ? remainingAmount : averageAmount;
            _paymentPlans.Add(new PaymentPlan(planDates[i], currentAmount, $"第{i + 1}期"));
            remainingAmount -= currentAmount;
        }
    }

    public void StartExecution()
    {
        if (Status != ContractStatus.Draft) throw new BusinessException("只有草稿合同可以开始执行");
        Status = ContractStatus.Executing;
    }

    public void Complete()
    {
        if (Status != ContractStatus.Executing) throw new BusinessException("只有执行中合同可以完成");
        Status = ContractStatus.Completed;
        AddDomainEvent(new ContractCompletedEvent(Id, ContractNo, DateTime.Now));
    }

    public void Cancel(string? reason = null)
    {
        if (Status is not (ContractStatus.Draft or ContractStatus.Executing))
        {
            throw new BusinessException("当前合同状态不允许作废");
        }
        if (string.IsNullOrWhiteSpace(reason)) throw new BusinessException("作废合同必须填写原因");

        Status = ContractStatus.Cancelled;
        Remark = reason.Trim();
        AddDomainEvent(new ContractCancelledEvent(Id, ContractNo, Remark, DateTime.Now));
    }

    public void Terminate(string? reason = null)
    {
        if (Status != ContractStatus.Executing) throw new BusinessException("只有执行中合同可以终止");
        Status = ContractStatus.Terminated;
        Remark = string.IsNullOrWhiteSpace(reason) ? Remark : reason.Trim();
    }

    public bool CanEdit()
    {
        return Status is ContractStatus.Draft or ContractStatus.Executing;
    }

    public bool CanRecordPayment()
    {
        return Status is ContractStatus.Draft or ContractStatus.Executing;
    }

    public void RecordPayment(int planId, decimal amount, DateTime actualDate)
    {
        if (!CanRecordPayment()) throw new BusinessException("当前合同状态不允许登记回款");

        var plan = _paymentPlans.FirstOrDefault(p => p.Id == planId);
        if (plan == null) throw new BusinessException("回款计划不存在");

        if (Status == ContractStatus.Draft)
        {
            StartExecution();
        }

        plan.RecordActualPayment(amount, actualDate);
        AddDomainEvent(new PaymentRecordedEvent(Id, plan.Id, amount, actualDate, DateTime.Now));
        TryCompleteByPayments();
    }

    public void RefreshOverduePaymentPlans(DateTime today)
    {
        foreach (var plan in _paymentPlans)
        {
            plan.MarkOverdue(today);
        }
    }

    public void SetOwner(int? ownerUserId)
    {
        if (ownerUserId.HasValue && ownerUserId.Value <= 0) throw new BusinessException("负责人用户ID无效");

        OwnerUserId = ownerUserId;
    }

    private void TryCompleteByPayments()
    {
        if (_paymentPlans.Count > 0 && _paymentPlans.All(p => p.Status == PaymentPlanStatus.Paid))
        {
            if (Status == ContractStatus.Draft)
            {
                StartExecution();
            }

            if (Status == ContractStatus.Executing)
            {
                Complete();
            }
        }
    }

    private void EnsurePaymentPlanTotal(decimal amountToAdd)
    {
        if (_paymentPlans.Sum(p => p.PlanAmount) + amountToAdd > TotalAmount)
        {
            throw new BusinessException("回款计划累计金额不能超过合同总金额");
        }
    }
}
