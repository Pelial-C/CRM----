using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Contracts;

public class ContractItem : Entity<int>
{
    public int ContractId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }

    protected ContractItem() { }

    public ContractItem(string productName, int quantity, decimal unitPrice)
    {
        Update(productName, quantity, unitPrice);
    }

    public void Update(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName)) throw new BusinessException("产品或服务名称不能为空");
        if (quantity <= 0) throw new BusinessException("数量必须大于0");
        if (unitPrice <= 0) throw new BusinessException("单价必须大于0");

        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        Subtotal = quantity * unitPrice;
    }
}
