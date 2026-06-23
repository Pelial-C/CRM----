using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Contracts;

public class Contract : AggregateRoot<int>
{
    public string ContractNo { get; private set; }
    public string ContractName { get; private set; }
    public string CabinetNo { get; private set; } // 合同柜编号

    // 关联客户 (甲方)
    public int CustomerId { get; private set; }
    public string PartyAName { get; private set; }

    // 关联联系人
    public int ContactId { get; private set; }
    public string ContactName { get; private set; }

    // 组织归属
    public string RegionalCompany { get; private set; }
    public string AffiliatedCompany { get; private set; }

    // 业务属性
    public ServiceType ServiceType { get; private set; }
    public ContractType ContractType { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int WarningDays { get; private set; }
    public decimal TotalAmount { get; private set; }
    public PaymentFrequency PaymentFrequency { get; private set; }

    // 内部集合
    private readonly List<ContractItem> _items = new();
    public IReadOnlyCollection<ContractItem> Items => _items.AsReadOnly();

    private readonly List<PaymentPlan> _paymentPlans = new();
    public IReadOnlyCollection<PaymentPlan> PaymentPlans => _paymentPlans.AsReadOnly();

    protected Contract() { }

    public Contract(
        string contractNo, string contractName, int customerId, string partyAName,
        int contactId, string contactName, decimal totalAmount,
        DateTime startDate, DateTime endDate, PaymentFrequency frequency,
        string regionalCompany, string affiliatedCompany, ServiceType serviceType)
    {
        if (totalAmount <= 0) throw new BusinessException("合同金额必须大于0");
        if (endDate <= startDate) throw new BusinessException("终止时间必须晚于起始时间");

        ContractNo = contractNo;
        ContractName = contractName;
        CustomerId = customerId;
        PartyAName = partyAName;
        ContactId = contactId;
        ContactName = contactName;
        TotalAmount = totalAmount;
        StartDate = startDate;
        EndDate = endDate;
        PaymentFrequency = frequency;
        RegionalCompany = regionalCompany;
        AffiliatedCompany = affiliatedCompany;
        ServiceType = serviceType;

        ContractType = ContractType.NewSign;
        WarningDays = 30;
        CabinetNo = "UNASSIGNED";
    }

    // 充血行为：添加明细
    public void AddItem(string productName, int quantity, decimal unitPrice)
    {
        _items.Add(new ContractItem(productName, quantity, unitPrice));
    }

    // 充血行为：自动生成回款计划
    public void GeneratePaymentPlans()
    {
        if (_paymentPlans.Any()) throw new BusinessException("回款计划已生成，请勿重复操作。");

        int months = (int)PaymentFrequency;
        DateTime currentDate = StartDate;
        decimal amountPerPeriod = Math.Round(TotalAmount / (12 / (decimal)months), 2);
        decimal remainingAmount = TotalAmount;
        int period = 1;

        while (currentDate < EndDate)
        {
            currentDate = currentDate.AddMonths(months);
            if (currentDate > EndDate) currentDate = EndDate;

            decimal currentAmount = amountPerPeriod;
            if (period * amountPerPeriod > TotalAmount) // 处理除不尽的尾数
            {
                currentAmount = remainingAmount;
            }

            _paymentPlans.Add(new PaymentPlan(currentDate, currentAmount, $"第{period}期"));
            remainingAmount -= currentAmount;
            period++;
        }
    }
}
