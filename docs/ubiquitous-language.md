# CRM Lite 统一语言

| 英文名 | 中文名 | 所属上下文 | 当前代码位置 | 说明 |
| --- | --- | --- | --- | --- |
| Customer | 客户 | 客户上下文 | `CRM.Domain/Customers/Customer.cs` | 企业维护的客户资料，是业务客户档案，不是登录账号 |
| Contact | 联系人 | 客户上下文 | `CRM.Domain/Customers/Contact.cs` | 隶属于某个 Customer 的联系人，可标记为关键决策人 |
| Contract | 合同 | 合同上下文 | `CRM.Domain/Contracts/Contract.cs` | 合同聚合根，维护合同基础信息、明细、回款计划和状态 |
| ContractItem | 合同明细 | 合同上下文 | `CRM.Domain/Contracts/ContractItem.cs` | 合同中的产品或服务明细，包含数量、单价、小计 |
| PaymentPlan | 回款计划 | 合同上下文 | `CRM.Domain/Contracts/PaymentPlan.cs` | 合同下的计划回款节点，同时记录实际回款金额和状态 |
| ActualPayment | 实际回款 | 合同上下文 | `PaymentPlan.ActualAmount`、`PaymentPlan.ActualDate` | 当前未单独建实体，由 PaymentPlan 保存累计实际回款金额和最近回款日期 |
| ContractStatus | 合同状态 | 合同上下文 | `CRM.Domain/Contracts/ContractStatus.cs` | 合同生命周期状态：Draft、Executing、Completed、Cancelled、Terminated |
| PaymentPlanStatus | 回款计划状态 | 合同上下文 | `CRM.Domain/Contracts/PaymentPlanStatus.cs` | 回款计划状态：Pending、PartialPaid、Paid、Overdue |
| AppUser | 系统用户 | 用户权限上下文 | `CRM.Domain/Users/AppUser.cs` | 可登录系统的账号，保存用户名、密码哈希、角色、启停用和关联客户 |
| CustomerUser | 外部客户用户 | 用户权限上下文 | `CRM.Domain.Shared/Enums/UserRole.cs` | AppUser 的一种角色，用于外部客户登录客户门户 |
| Sales | 销售用户 | 用户权限上下文 | `CRM.Domain.Shared/Enums/UserRole.cs` | AppUser 的一种角色，维护自己负责的客户、联系人、合同和回款 |
| Admin | 管理员 | 用户权限上下文 | `CRM.Domain.Shared/Enums/UserRole.cs` | AppUser 的一种角色，拥有系统最高管理权限 |
| EnterpriseUser | 企业普通用户 | 用户权限上下文 | `CRM.Domain.Shared/Enums/UserRole.cs` | 企业内部普通用户，主要查看客户、合同和回款信息 |
| OwnerUserId | 负责人用户 ID | 客户/合同上下文 | `Customer.OwnerUserId`、`Contract.OwnerUserId` | 记录客户或合同归属的销售用户 ID，用于 Sales 数据隔离 |
| RelatedCustomerId | 关联客户 ID | 用户权限上下文 | `AppUser.RelatedCustomerId` | CustomerUser 绑定的 Customer ID，用于客户门户数据过滤 |
| RelatedSalesUserId | 关联销售 ID | 用户权限上下文 | `AppUser.RelatedSalesUserId` | 普通用户或客户用户可绑定负责销售用户 |

## Customer 与 CustomerUser 的区别

`Customer` 是企业在 CRM 系统中维护的客户资料，例如“某某科技有限公司”，它属于客户上下文，是被管理的业务对象。

`CustomerUser` 是 `AppUser.Role` 的一个枚举值，表示“可以登录系统的外部客户账号”。它属于用户权限上下文，是登录主体。

二者不是同一个概念。一个 Customer 可以没有 CustomerUser；一个 CustomerUser 必须通过 `RelatedCustomerId` 才能绑定到某个 Customer 并查看该客户相关数据。
