# 第1次开发 - 领域层（CRM.Domain）

**日期**：2026-06-27
**开发层**：CRM.Domain（领域层）
**涉及模块**：客户模块 + 联系人模块

---

## 修改文件

### 1. `CRM.Domain/Customers/Customer.cs`

**新增方法：**

- `UpdateContact(int contactId, string name, string? phone, string? title, string? email, bool isKeyDecisionMaker)`
  - 通过聚合根修改联系人信息，符合 DDD 聚合一致性要求
  - 包含业务规则：已删除客户不能维护联系人
  - 包含业务规则：同一客户下联系人姓名不能重复（排除自身）
  - 内部调用 `Contact.Update()` 执行联系人自身的校验和赋值

- `CanDelete(bool hasContracts)`
  - 判断客户是否可以物理删除
  - 若有关联合同则返回 false，禁止物理删除
  - 参数由应用层传入（跨聚合查询不属于领域层职责）

**已有方法确认：**

| 方法 | 对应业务规则 |
|------|-------------|
| `UpdateInfo()` | 客户名称不能为空、自动 Trim |
| `AddContact()` | 已删除客户不能维护联系人、同客户下联系人不能重名 |
| `RemoveContact()` | 联系人不存在时抛异常 |
| `MarkAsDeleted()` | 逻辑删除客户 |

### 2. `CRM.Domain/Customers/Contact.cs`

**无需修改。** 已覆盖需求文档中所有应放在领域层的规则：

| 规则 | 实现位置 |
|------|---------|
| 联系人必须归属于客户 | 构造函数校验 CustomerId > 0 |
| 联系人姓名不能为空 | `Update()` 中校验 |
| 联系人更新时做 Trim | `Update()` 中处理 |

手机号/邮箱格式校验由 Application.Contracts 层 DTO 的 `[Phone]` `[EmailAddress]` 注解实现，不放在领域层。

---

## 领域层业务规则覆盖情况

| 规则 | 状态 |
|------|------|
| 客户名称不能为空 | ✅ Customer.UpdateInfo() |
| 客户信息更新时做 Trim | ✅ Customer.UpdateInfo() |
| 已删除客户不能维护联系人 | ✅ Customer.AddContact() + UpdateContact() |
| 联系人必须归属于客户 | ✅ Contact 构造函数 |
| 联系人姓名不能为空 | ✅ Contact.Update() |
| 同一客户下联系人不能重名 | ✅ Customer.AddContact() + UpdateContact() |
| 联系人更新时做 Trim | ✅ Contact.Update() |
| 逻辑删除客户 | ✅ Customer.MarkAsDeleted() |
| 客户有关联合同时不能物理删除 | ✅ Customer.CanDelete() |
