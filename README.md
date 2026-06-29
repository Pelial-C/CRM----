# 企业客户与合同管理系统 CRM Lite

CRM Lite 是一个 ASP.NET Core MVC 课程设计项目，用于制造企业统一管理客户、联系人、合同、合同明细和回款计划，替代 Excel 台账，提高数据一致性和查询效率。

## 技术栈

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB
- DDD 分层架构
- Bootstrap / jQuery

## 分层结构

- `CRM.Domain`：聚合根、实体、值对象、领域规则
- `CRM.Domain.Shared`：共享枚举、异常等基础类型
- `CRM.Application.Contracts`：DTO 和应用服务接口
- `CRM.Application`：应用服务实现
- `CRM.Infrastructure`：EF Core DbContext、Repository、Migrations
- `CRM.Web`：MVC Controller、Views、Program.cs

## 核心模块

- 客户管理
- 联系人管理
- 合同管理
- 回款计划管理

## 数据库初始化

```bash
cd CRM.Lite
dotnet restore
dotnet ef database update --project CRM.Infrastructure --startup-project CRM.Web
dotnet run --project CRM.Web
```

默认连接字符串使用 SQL Server LocalDB：`CrmLiteDb`。

## 演示流程

1. 新增客户。
2. 新增联系人。
3. 进入合同管理并新增合同。
4. 选择客户后联动加载联系人。
5. 添加合同明细并保存合同。
6. 进入合同详情。
7. 自动生成回款计划或手工新增回款计划。
8. 登记实际回款。
9. 查看回款状态和合同完成状态。

## 当前限制

- 权限模块暂未完整实现。
- 基础数据暂以枚举和页面下拉方式实现，未做后台字典维护。

## 成员分工

- 领域建模师：负责客户聚合、合同聚合、回款计划领域规则、状态流转、EF Core Code First 映射与领域模型说明。
