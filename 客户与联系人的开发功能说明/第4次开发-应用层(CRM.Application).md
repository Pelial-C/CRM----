# 第4次开发 - 应用层（CRM.Application）

**日期**：2026-06-27
**开发层**：CRM.Application（应用层）
**涉及模块**：客户模块 + 联系人模块

---

## 核心设计思想

应用层是**编排者**，职责：
1. 接收 DTO → 组装参数
2. 调用领域方法 → 业务规则自动执行
3. 调用仓储 → 持久化

**不在应用层写业务规则**，所有业务规则由领域层方法封装。

---

## 新建文件

### 1. `CRM.Application/Contacts/ContactAppService.cs`

联系人应用服务，实现 `IContactAppService` 接口。

| 方法 | 实现逻辑 | 调用的领域方法 |
|------|---------|---------------|
| GetListAsync | 通过客户聚合根获取联系人，内存筛选姓名/职务 | — |
| GetByIdAsync | 通过 Contact 仓储获取 | — |
| CreateAsync | 检查客户存在 → customer.AddContact() → 保存 | Customer.AddContact() |
| UpdateAsync | 获取联系人 → 获取客户 → customer.UpdateContact() → 保存 | Customer.UpdateContact() |
| DeleteAsync | 获取联系人 → 获取客户 → customer.RemoveContact() → 保存 | Customer.RemoveContact() |

**关键设计：联系人的增删改全部通过 Customer 聚合根操作**

因为 Contact 是 Customer 聚合内的实体，所有修改必须经过聚合根，保证聚合一致性。虽然 Contact 有独立的 `_contactRepo`，但仅用于 `GetByIdAsync` 获取单个联系人信息（如获取 CustomerId），不用于增删改。

---

## 修改文件

### 2. `CRM.Application/Customers/CustomerAppService.cs`

**改动：** 完全重写，对比原版的主要变化：

| 原版 | 新版 | 变化 |
|------|------|------|
| GetListAsync(string? keyword) | GetPagedListAsync(CustomerQueryDto) | 改为分页+多条件筛选 |
| — | GetDetailAsync(int id) | **新增**：客户详情含联系人+合同列表 |
| CreateAsync 无唯一性校验 | CreateAsync 先 CheckNameUniqueAsync | **新增**：企业名称唯一校验 |
| UpdateAsync 无唯一性校验 | UpdateAsync 先 CheckNameUniqueAsync(excludeId) | **新增**：编辑时排除自身 |
| DeleteAsync 直接物理删除 | DeleteAsync 判断 CanDelete | **新增**：有关联合同则逻辑删除 |
| 注入1个仓储 | 注入2个仓储 | **新增**：IRepository<Contract> 用于检查合同 |

**方法说明：**

- **CheckNameUniqueAsync(name, excludeId)**：私有方法，新增时 excludeId=null，编辑时传入自身 Id 排除。跨全库的唯一性查询属于应用层职责（不属于领域层）。
- **BuildPredicate(input)**：私有方法，将 CustomerQueryDto 转换为 Expression<Func<Customer, bool>>，用于分页查询的条件过滤。
- **MapToDto / MapContactToDto / MapContractToSummaryDto**：私有方法，手动映射实体到 DTO。

### 3. `CRM.Web/Program.cs`

**改动：** 注册 ContactAppService 依赖注入。

```csharp
builder.Services.AddScoped<IContactAppService, ContactAppService>();
```

---

## 应用层业务逻辑覆盖情况

| 需求文档要求的规则 | 实现位置 | 状态 |
|------------------|---------|------|
| 客户名称唯一 | CustomerAppService.CheckNameUniqueAsync() | ✅ |
| 客户有关联合同时不能物理删除 | CustomerAppService.DeleteAsync() → CanDelete() | ✅ |
| 客户删除优先使用逻辑删除 | CustomerAppService.DeleteAsync() → MarkAsDeleted() | ✅ |
| 被删除客户不能新增联系人 | Customer.AddContact() 领域规则 | ✅ |
| 被删除客户不能编辑联系人 | Customer.UpdateContact() 领域规则 | ✅ |
| 同客户下联系人不能重名 | Customer.AddContact() / UpdateContact() 领域规则 | ✅ |
| 客户详情展示联系人列表 | CustomerAppService.GetDetailAsync() → AutoInclude | ✅ |
| 客户详情展示合同列表 | CustomerAppService.GetDetailAsync() → contractRepo | ✅ |
| 列表查询支持筛选和分页 | GetPagedListAsync() → BuildPredicate + GetPagedAsync | ✅ |
| Controller 只调用应用服务 | 应用层封装所有逻辑 | ✅ |
| 应用服务调用领域模型方法 | AddContact / UpdateContact / RemoveContact / CanDelete / MarkAsDeleted | ✅ |
