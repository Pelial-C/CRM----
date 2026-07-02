# 第2次开发 - 应用抽象层（CRM.Application.Contracts）

**日期**：2026-06-27
**开发层**：CRM.Application.Contracts（应用抽象层）
**涉及模块**：客户模块 + 联系人模块

---

## 新建文件

### 1. `CRM.Application.Contracts/Common/Dtos/PagedResultDto.cs`

通用分页结果 DTO，供所有分页查询接口复用。

```csharp
public class PagedResultDto<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages   // 计算属性：总页数
    public bool HasPreviousPage  // 计算属性：是否有上一页
    public bool HasNextPage      // 计算属性：是否有下一页
}
```

### 2. `CRM.Application.Contracts/Customers/Dtos/CustomerQueryDto.cs`

客户查询条件 DTO，支持按名称、行业、创建时间范围筛选 + 分页。

| 字段 | 说明 |
|------|------|
| Name | 按企业名称模糊搜索 |
| Industry | 按行业筛选 |
| StartTime / EndTime | 按创建时间范围筛选 |
| PageIndex | 页码，默认1 |
| PageSize | 每页条数，默认10 |

### 3. `CRM.Application.Contracts/Customers/Dtos/CustomerDetailDto.cs`

客户详情 DTO，包含联系人列表和合同摘要列表。

| 字段 | 说明 |
|------|------|
| 基本信息字段 | 同 CustomerDto |
| Contacts | 联系人列表 List<ContactDto> |
| Contracts | 合同摘要列表 List<ContractSummaryDto> |

同时新建 `ContractSummaryDto`，用于在客户详情页展示合同列表（只含关键字段：Id、ContractNo、ContractName、TotalAmount、Status、SignDate）。

### 4. `CRM.Application.Contracts/Contacts/IContactAppService.cs`

联系人应用服务接口。

| 方法 | 说明 |
|------|------|
| GetListAsync(ContactQueryDto) | 查询某客户下联系人列表 |
| GetByIdAsync(int id) | 根据Id获取联系人 |
| CreateAsync(CreateContactDto) | 新增联系人 |
| UpdateAsync(UpdateContactDto) | 编辑联系人 |
| DeleteAsync(int id) | 删除联系人 |

### 5. `CRM.Application.Contracts/Contacts/Dtos/ContactDto.cs`

联系人查询结果 DTO，包含 Id、CustomerId、Name、Title、Phone、Email、IsKeyDecisionMaker。

### 6. `CRM.Application.Contracts/Contacts/Dtos/CreateContactDto.cs`

新增联系人 DTO，使用 DataAnnotations 做后端验证：

| 字段 | 验证 |
|------|------|
| CustomerId | [Required] |
| Name | [Required] [MaxLength(50)] |
| Title | [MaxLength(50)] |
| Phone | [Phone] [MaxLength(30)] |
| Email | [EmailAddress] [MaxLength(100)] |
| IsKeyDecisionMaker | bool |

### 7. `CRM.Application.Contracts/Contacts/Dtos/UpdateContactDto.cs`

编辑联系人 DTO，不含 CustomerId（联系人不能转移客户），其余验证同 CreateContactDto，增加 [Required] Id。

### 8. `CRM.Application.Contracts/Contacts/Dtos/ContactQueryDto.cs`

联系人查询条件 DTO，CustomerId 必填（联系人必须在客户上下文内查询），Name 和 Title 可选筛选。

---

## 修改文件

### 1. `CRM.Application.Contracts/Customers/ICustomerAppService.cs`

**改动：** 重写接口，替换原有的简单 keyword 查询，新增分页查询和详情查询。

| 原方法 | 新方法 | 变化 |
|--------|--------|------|
| GetListAsync(string? keyword) | GetPagedListAsync(CustomerQueryDto) | 改为分页+多条件筛选 |
| — | GetDetailAsync(int id) | **新增**：返回 CustomerDetailDto |
| GetByIdAsync(int id) | GetByIdAsync(int id) | 保留 |
| CreateAsync / UpdateAsync / DeleteAsync | 不变 | 保留 |

### 2. `CRM.Application.Contracts/Customers/Dtos/CustomerDto.cs`

**改动：** 新增 `Remark` 和 `IsDeleted` 字段，移除冗余注释。

### 3. `CRM.Application.Contracts/Customers/Dtos/CreateCustomerDto.cs`

**改动：**
- 移除 CreditCode 的 `[Required]`（需求文档中信用代码非必填）
- 新增 `[MaxLength(50)]` 对 CreditCode 的长度约束
- 新增 Industry 的 `[MaxLength(50)]`
- 新增 DetailAddress 的 `[MaxLength(200)]`
- 新增 Remark 字段 `[MaxLength(500)]`

---

## 接口设计对照需求文档

| 需求文档要求 | 接口实现 | 状态 |
|-------------|---------|------|
| 客户列表分页查询 | GetPagedListAsync(CustomerQueryDto) | ✅ |
| 按企业名称、行业、创建时间查询 | CustomerQueryDto: Name/Industry/StartTime/EndTime | ✅ |
| 客户详情含联系人+合同列表 | GetDetailAsync → CustomerDetailDto | ✅ |
| 联系人必须在客户上下文内管理 | ContactQueryDto.CustomerId [Required] | ✅ |
| 联系人按姓名、职务筛选 | ContactQueryDto: Name/Title | ✅ |
| 手机号/邮箱后端格式校验 | [Phone] / [EmailAddress] | ✅ |
