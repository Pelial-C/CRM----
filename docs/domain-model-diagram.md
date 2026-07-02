# CRM Lite 领域模型图说明

PlantUML 源文件位于 `docs/diagrams`，用于说明当前代码中的聚合、实体关系和状态流转。

| 图文件 | 图名称 | 说明 |
| --- | --- | --- |
| `docs/diagrams/domain-overview.puml` | 领域模型总览图 | 展示客户、合同、用户权限三个主要上下文之间的关系 |
| `docs/diagrams/customer-aggregate.puml` | Customer 聚合图 | 展示 Customer 聚合根、Contact 实体、Address 值对象和客户领域事件 |
| `docs/diagrams/contract-aggregate.puml` | Contract 聚合图 | 展示 Contract 聚合根、ContractItem、PaymentPlan 和合同领域事件 |
| `docs/diagrams/user-permission-context.puml` | 用户权限上下文图 | 展示 AppUser、AppRole、UserRole 以及 RelatedCustomerId、OwnerUserId 的含义 |
| `docs/diagrams/contract-lifecycle-state.puml` | 合同生命周期状态图 | 展示 Draft、Executing、Completed、Cancelled、Terminated 的状态流转 |
| `docs/diagrams/payment-plan-state.puml` | 回款计划状态图 | 展示 Pending、PartialPaid、Paid、Overdue 的状态流转 |
| `docs/diagrams/entity-relationship.puml` | ER 图 | 展示数据库表、主要字段、主外键关系和删除约束 |
| `docs/diagrams/system-architecture.puml` | 系统架构图 | 展示 React 前端、ASP.NET Core 后端、应用层、领域层、基础设施层与数据库之间的依赖 |

这些图没有生成图片文件，提交的是可维护的 `.puml` 源文件。后续可以用 PlantUML 插件、命令行或在线渲染工具生成 PNG/SVG。
