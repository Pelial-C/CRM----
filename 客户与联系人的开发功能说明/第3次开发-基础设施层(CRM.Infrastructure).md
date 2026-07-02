# 第3次开发 - 基础设施层（CRM.Infrastructure）+ 仓储接口（CRM.Domain）

**日期**：2026-06-27
**开发层**：CRM.Infrastructure + CRM.Domain/Repositories
**涉及模块**：客户模块 + 联系人模块

---

## 设计思路

需求文档要求"基础设施层已经完成时，尽量不要改"，但现有仓储无法满足以下需求：
- 分页查询（需求：默认每页10-20条）
- 按条件筛选客户（需求：按企业名称、行业、创建时间查询）
- 判断客户是否有关联合同（需求：有关联合同时禁止物理删除）

因此只在通用仓储上**扩展查询方法**，不写任何业务规则。

---

## 修改文件

### 1. `CRM.Domain/Repositories/IRepository.cs`

**改动：** 在保留原有5个方法的基础上，新增4个方法。

| 新增方法 | 签名 | 用途 |
|---------|------|------|
| GetListAsync | `Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)` | 条件过滤查询 |
| GetPagedAsync | `Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(predicate, pageIndex, pageSize)` | 分页+条件查询 |
| AnyAsync | `Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)` | 存在性检查 |
| FirstOrDefaultAsync | `Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)` | 按条件查单个 |

**为什么返回元组而不是 PagedResultDto？**

`PagedResultDto` 定义在 `CRM.Application.Contracts` 层，而 `IRepository` 在 `CRM.Domain` 层。Domain 层不能依赖 Application.Contracts 层（会产生循环依赖），因此仓储返回原始元组 `(Items, TotalCount)`，由 Application 层组装为 `PagedResultDto`。

**应用层使用示例：**

```csharp
// 分页查询客户
var (items, totalCount) = await _customerRepo.GetPagedAsync(predicate, query.PageIndex, query.PageSize);

// 检查客户是否有关联合同
var hasContracts = await _contractRepo.AnyAsync(c => c.CustomerId == customerId);

// 条件过滤查询合同（客户详情页用）
var contracts = await _contractRepo.GetListAsync(c => c.CustomerId == customerId);
```

### 2. `CRM.Infrastructure/Repositories/EfRepository.cs`

**改动：** 实现 IRepository 新增的4个方法。

| 方法 | 实现 |
|------|------|
| GetListAsync(predicate) | `_dbSet.Where(predicate).ToListAsync()` |
| GetPagedAsync | 先 `CountAsync` 获取总数，再 `Skip/Take` 分页 |
| AnyAsync | `_dbSet.AnyAsync(predicate)` |
| FirstOrDefaultAsync | `_dbSet.FirstOrDefaultAsync(predicate)` |

原有方法（GetByIdAsync、GetListAsync、InsertAsync、UpdateAsync、DeleteAsync）保持不变。

### 3. `CRM.Infrastructure/Persistence/CrmDbContext.cs`

**无需修改。**

| 需求 | 现有配置 | 说明 |
|------|---------|------|
| 查询客户时加载联系人 | `entity.Navigation(c => c.Contacts).AutoInclude()` | 已配置 ✅ |
| 查询某客户的合同 | 通过 `IRepository<Contract, int>.GetListAsync(predicate)` | 无需修改 ✅ |
| 判断合同是否存在 | 通过 `IRepository<Contract, int>.AnyAsync(predicate)` | 无需修改 ✅ |

---

## 仓储方法与业务需求对应关系

| 业务需求 | 使用的仓储方法 |
|---------|--------------|
| 客户列表分页+筛选 | `GetPagedAsync(predicate, pageIndex, pageSize)` |
| 客户名称唯一性校验 | `AnyAsync(c => c.Name == name)` |
| 客户详情-联系人列表 | AutoInclude + Customer.Contacts |
| 客户详情-合同列表 | `GetListAsync<Contract>(c => c.CustomerId == id)` |
| 删除客户前-检查合同 | `AnyAsync<Contract>(c => c.CustomerId == id)` |
| 联系人按姓名/职务筛选 | 条件过滤在 Application 层组装 predicate |
