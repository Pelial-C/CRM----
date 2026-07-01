using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;

namespace CRM.Domain.Users;

public class AppUser : AggregateRoot<int>
{
    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public int? RelatedCustomerId { get; private set; }
    public int? RelatedSalesUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected AppUser()
    {
    }

    public AppUser(string userName, string displayName, UserRole role, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new BusinessException("用户名不能为空");
        if (string.IsNullOrWhiteSpace(displayName)) throw new BusinessException("显示名称不能为空");

        UserName = userName.Trim();
        DisplayName = displayName.Trim();
        Role = role;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        IsActive = true;
        CreatedAt = DateTime.Now;
        UpdatedAt = CreatedAt;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new BusinessException("密码哈希不能为空");

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.Now;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.Now;
    }

    public void SetRelatedCustomer(int? customerId)
    {
        if (customerId.HasValue && customerId.Value <= 0) throw new BusinessException("关联客户ID无效");

        RelatedCustomerId = customerId;
        UpdatedAt = DateTime.Now;
    }

    public void SetRelatedSalesUser(int? salesUserId)
    {
        if (salesUserId.HasValue && salesUserId.Value <= 0) throw new BusinessException("关联销售ID无效");

        RelatedSalesUserId = salesUserId;
        UpdatedAt = DateTime.Now;
    }
}
