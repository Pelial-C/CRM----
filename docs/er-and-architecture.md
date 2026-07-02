# CRM Lite ER 图与系统架构图

本文档补充数据库实体关系与系统架构视角，和 `docs/domain-model-diagram.md` 中的领域模型图形成互补。

## 图文件

| 图文件 | 图名称 | 说明 |
| --- | --- | --- |
| `docs/diagrams/entity-relationship.puml` | ER 图 | 展示数据库表、主要字段、主外键关系和删除约束 |
| `docs/diagrams/system-architecture.puml` | 系统架构图 | 展示 React 前端、ASP.NET Core Web/API、应用层、领域层、基础设施层与数据库之间的依赖 |

## 说明

- ER 图以 `CrmDbContext` 的 EF Core 映射为依据，覆盖客户、联系人、合同、合同明细、回款计划、用户、角色等核心表。
- 系统架构图反映当前项目的混合架构：`CRM.Client` 提供 React 前端，`CRM.Web` 同时提供 MVC/Razor 页面、登录认证和 API。
- 图文件使用 PlantUML 源文件保存，可用 PlantUML 插件、命令行或在线工具渲染为 PNG/SVG。
