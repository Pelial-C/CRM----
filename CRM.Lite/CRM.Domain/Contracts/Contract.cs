using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;

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
        string? remark = null,
        string? cabinetNo = null,
        int? warningDays = null)
    {
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
            remark,
            cabinetNo,
            warningDays);

        Status = ContractStatus.Draft;
        if (!warningDays.HasValue) WarningDays = 30;
        if (string.IsNullOrWhiteSpace(cabinetNo)) CabinetNo = "UNASSIGNED";
    }

    public Contract(
        string contractNo,
        string contractName,
        int customerId,
        string customerName,
        int contactId,
        string contactName,
        decimal totalAmount,
        DateTime startDate,
        DateTime endDate,
        PaymentFrequency frequency,
        string regionalCompany,
        string affiliatedCompany,
        ServiceType serviceType)
        : this(
            contractNo,
            contractName,
            customerId,
            customerName,
            contactId,
            contactName,
            totalAmount,
            DateTime.Today,
            startDate,
            endDate,
            frequency,
            regionalCompany,
            affiliatedCompany,
            serviceType)
    {
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
        string? remark = null,
        string? cabinetNo = null,
        int? warningDays = null)
    {
        if (string.IsNullOrWhiteSpace(contractNo)) throw new BusinessException("合同编号不能为空");
        if (string.IsNullOrWhiteSpace(contractName)) throw new BusinessException("合同名称不能为空");
        if (customerId <= 0) throw new BusinessException("合同必须关联客户");
        if (string.IsNullOrWhiteSpace(customerName)) throw new BusinessException("客户名称不能为空");
        if (contactId.HasValue && contactId.Value <= 0) throw new BusinessException("联系人Id必须有效");
        if (totalAmount <= 0) throw new BusinessException("合同金额必须大于0");
        if (endDate < startDate) throw new BusinessException("合同结束日期不能早于开始日期");

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
        Remark = string.IsNullOrWhiteSpace(remark) ? null : remark.Trim();
        if (!string.IsNullOrWhiteSpace(cabinetNo)) CabinetNo = cabinetNo.Trim();
        if (warningDays.HasValue) WarningDays = warningDays.Value;
    }

    public ContractItem AddItem(string productName, int quantity, decimal unitPrice)
    {
        var item = new ContractItem(productName, quantity, unitPrice);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(int itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new BusinessException("合同明细不存在");

        _items.Remove(item);
    }

    public PaymentPlan AddPaymentPlan(DateTime planDate, decimal planAmount, string? description = null)
    {
        EnsurePaymentPlanTotal(planAmount);

        var paymentPlan = new PaymentPlan(planDate, planAmount, description);
        _paymentPlans.Add(paymentPlan);
        return paymentPlan;
    }

    public void GeneratePaymentPlans()
    {
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

    public void ChangeStatus(ContractStatus status)
    {
        Status = status;
    }

    public void Cancel(string? reason = null)
    {
        Status = ContractStatus.Cancelled;
        Remark = string.IsNullOrWhiteSpace(reason) ? Remark : reason.Trim();
    }

    public void Terminate(string? reason = null)
    {
        Status = ContractStatus.Terminated;
        Remark = string.IsNullOrWhiteSpace(reason) ? Remark : reason.Trim();
    }

    public void ReplaceItems(IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        _items.Clear();
        foreach (var (productName, quantity, unitPrice) in items)
        {
            AddItem(productName, quantity, unitPrice);
        }
    }

    public void RecordPayment(int planId, decimal actualAmount, DateTime actualDate)
    {
        if (Status is ContractStatus.Cancelled or ContractStatus.Terminated)
        {
            throw new BusinessException("已作废或已终止的合同不能录入回款");
        }

        var plan = _paymentPlans.FirstOrDefault(p => p.Id == planId);
        if (plan == null) throw new BusinessException("未找到指定的回款计划");

        var totalAfterPayment = _paymentPlans.Sum(p => p.ActualAmount) + actualAmount;
        if (totalAfterPayment > TotalAmount)
        {
            throw new BusinessException("累计实际回款不能超过合同总金额");
        }

        plan.RecordActualPayment(actualAmount, actualDate);

        if (_paymentPlans.All(p => p.Status == PaymentPlanStatus.Paid))
        {
            ChangeStatus(ContractStatus.Completed);
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
