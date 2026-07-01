# CRM Lite 领域模型设计

## 1. 项目领域背景

CRM Lite 是企业内部客户合同管理系统，核心目标是把客户资料、联系人、合同、合同明细、回款计划和实际回款从 Excel 台账中抽象为可维护、可校验、可追踪的领域模型。系统当前采用 ASP.NET Core MVC + EF Core + DDD 分层结构，领域层位于 `CRM.Lite/CRM.Domain`，领域对象通过应用服务暴露给 MVC 和 API 层。

系统中的“客户”不是登录用户，而是企业维护的业务客户资料；可登录系统的账号由 `AppUser` 表示。外部客户登录账号使用 `CustomerUser` 角色，并通过 `RelatedCustomerId` 绑定客户资料。

## 2. 限界上下文划分

| 限界上下文 | 主要对象 | 说明 |
| --- | --- | --- |
| 客户上下文 | `Customer`、`Contact`、`Address` | 管理客户档案、客户地址、客户联系人和关键决策人信息 |
| 合同上下文 | `Contract`、`ContractItem`、`PaymentPlan` | 管理合同生命周期、合同明细、回款计划、实际回款登记 |
| 用户权限上下文 | `AppUser`、`AppRole`、`UserRole` | 管理系统用户、角色、登录端分流和数据归属 |
| 基础枚举上下文 | `PaymentFrequency`、`ServiceType`、`ContractType` | 为合同业务提供稳定枚举值 |

当前项目没有单独的外部客户自助业务上下文实体，`CustomerPortal` 只是基于 `AppUser.RelatedCustomerId` 读取客户关联数据。

## 3. 统一语言

统一语言的完整表见 `docs/ubiquitous-language.md`。关键约定如下：

- Customer：企业维护的客户资料，是客户上下文聚合根。
- CustomerUser：外部客户登录账号角色，是 `AppUser.Role` 的一种，不等同于 Customer。
- Contract：合同上下文聚合根，负责维护合同状态、明细和回款计划。
- PaymentPlan：回款计划实体，同时保存已登记实际回款金额和状态。
- OwnerUserId：客户或合同的销售负责人用户 ID，用于 Sales 数据隔离。

## 4. 聚合根设计

### Customer 聚合

`Customer` 是客户聚合根，负责维护客户基础信息和联系人集合。其内部通过 `_contacts` 管理 `Contact`，外部只能通过 `AddContact`、`RemoveContact` 等行为维护联系人，避免绕过“已删除客户不能维护联系人”和“同一客户下联系人姓名 + 手机号不能重复”的规则。

### Contract 聚合

`Contract` 是合同聚合根，内部维护 `_items` 和 `_paymentPlans`。合同明细和回款计划不能作为独立聚合随意修改，应通过合同聚合根进行维护，保证合同金额、明细合计、合同状态、回款状态之间的一致性。

### AppUser / AppRole 聚合

`AppUser` 表示可登录系统的用户，承担认证和端入口分流所需的角色、启停用、关联客户、关联销售字段。`AppRole` 用于描述系统角色元数据。

## 5. 实体设计

| 实体 | 所属聚合 | 关键字段 | 关键行为 |
| --- | --- | --- | --- |
| `Customer` | Customer | `Name`、`CreditCode`、`Address`、`IsDeleted`、`OwnerUserId` | `UpdateInfo`、`AddContact`、`MarkAsDeleted`、`SetOwner` |
| `Contact` | Customer | `CustomerId`、`Name`、`Phone`、`Email`、`IsKeyDecisionMaker` | `Update` |
| `Contract` | Contract | `ContractNo`、`TotalAmount`、`Status`、`CustomerId`、`OwnerUserId` | `ReplaceItems`、`AddPaymentPlan`、`RecordPayment`、`Cancel`、`Complete` |
| `ContractItem` | Contract | `ProductName`、`Quantity`、`UnitPrice`、`Subtotal` | `Update` |
| `PaymentPlan` | Contract | `PlanDate`、`PlanAmount`、`ActualAmount`、`Status` | `RecordActualPayment`、`MarkOverdue` |
| `AppUser` | User | `UserName`、`PasswordHash`、`Role`、`IsActive`、`RelatedCustomerId` | `SetPasswordHash`、`SetActive`、`SetRelatedCustomer` |
| `AppRole` | User | `Role`、`Name`、`Description` | 构造时建立角色描述 |

## 6. 值对象设计

当前显式值对象为 `Address`，包含省、市、区、详细地址。它没有独立身份，作为 `Customer` 的拥有类型持久化到客户表字段中。`Address.Empty` 用于避免客户地址为空引用。

后续可考虑将合同金额、统一社会信用代码、手机号、邮箱等规则进一步抽象为值对象，但当前课程设计版本保留为实体字段和 DataAnnotations 校验。

## 7. 领域服务设计

当前项目领域规则主要落在聚合根行为内，应用服务承担跨聚合查询与编排：

- `CustomerAppService`：负责客户名称、统一社会信用代码重复检查，以及删除前合同关联检查。
- `ContactAppService`：负责联系人创建前客户存在性检查、删除前合同引用检查。
- `ContractAppService`：负责客户和联系人归属检查、合同编号唯一性检查，并调用 `Contract` 聚合行为维护合同明细、回款计划和实际回款。

当前没有单独的 `DomainService` 类型。若后续规则需要跨多个聚合且不适合放入某个聚合根，例如“客户信用评级影响合同审批策略”，可新增领域服务。

## 8. 领域事件设计

`AggregateRoot<TKey>` 现在包含 `DomainEvents`、`AddDomainEvent`、`ClearDomainEvents`，仅在领域层记录事件，不接入 MediatR，也不在 Infrastructure 中发布。

已定义事件：

- `CustomerCreatedEvent`
- `ContactAddedEvent`
- `ContractCreatedEvent`
- `PaymentRecordedEvent`
- `ContractCompletedEvent`
- `ContractCancelledEvent`

事件用于表达领域内已经发生的重要事实，例如合同完成、合同作废、回款登记。后续可在应用服务保存聚合后统一读取事件并发布通知、写操作日志或触发消息。

## 9. 业务规则说明

完整规则矩阵见 `docs/business-rules.md`。核心规则包括：

- 客户名称不能为空。
- 已删除客户不能新增或维护联系人。
- 同一客户下联系人姓名 + 手机号不能重复。
- 合同编号不能为空，合同金额必须大于 0。
- 合同结束日期不能早于开始日期。
- 合同明细至少一项，且明细金额总和必须等于合同总金额。
- 合同作废必须填写原因。
- 已取消合同不能登记回款。
- 实际回款不能超过计划金额。
- 所有回款计划结清后合同自动完成。
- 未支付且计划日期早于当前日期的回款计划可以标记为逾期。

## 10. 领域模型图说明

PlantUML 图存放在 `docs/diagrams`：

- `domain-overview.puml`：领域模型总览。
- `customer-aggregate.puml`：Customer 聚合。
- `contract-aggregate.puml`：Contract 聚合。
- `user-permission-context.puml`：用户权限上下文。
- `contract-lifecycle-state.puml`：合同生命周期状态。
- `payment-plan-state.puml`：回款计划状态。

图的索引和说明见 `docs/domain-model-diagram.md`。

## 11. 领域规则测试说明

领域测试位于 `CRM.Lite/CRM.Domain.Tests`，按聚合拆分：

- `CustomerDomainTests.cs`：客户名称、删除后维护联系人、联系人重复规则。
- `ContractDomainTests.cs`：合同编号、金额、日期、明细、作废、取消后回款规则。
- `PaymentPlanDomainTests.cs`：实际回款上限、全部回款完成后合同完成、逾期标记规则。

这些测试直接调用领域对象，不依赖数据库、Controller 或应用服务，用于证明核心领域规则可以脱离 UI 和 EF Core 独立运行。
