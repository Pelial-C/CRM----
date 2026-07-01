# 企业客户与合同管理系统 CRM Lite

CRM Lite 是一个课程设计项目，用于替代 Excel 台账，集中管理客户、联系人、合同、合同明细和回款计划，并提供基础统计看板。

## 技术栈

- ASP.NET Core Web API
- EF Core
- SQL Server LocalDB
- DDD 分层架构
- React
- Vite
- TypeScript
- Axios
- React Router
- Bootstrap

## 后端结构

- `CRM.Domain`：领域实体、聚合根、领域规则
- `CRM.Domain.Shared`：枚举、业务异常
- `CRM.Application.Contracts`：DTO、应用服务接口、数据注解
- `CRM.Application`：应用服务实现
- `CRM.Infrastructure`：EF Core DbContext、仓储、迁移
- `CRM.Web`：Web API 控制器、CORS、Swagger、启动配置

## 前端结构

- `CRM.Client/src/api`：Axios 实例和 API 封装
- `CRM.Client/src/components`：布局、导航、表单、加载和错误组件
- `CRM.Client/src/pages`：看板、客户、联系人、合同和系统说明页面
- `CRM.Client/src/types`：前端 TypeScript 类型
- `CRM.Client/src/utils`：枚举、金额、日期格式化工具

## 后端启动

```powershell
cd CRM.Lite
dotnet restore
dotnet ef database update --project CRM.Infrastructure --startup-project CRM.Web
dotnet run --project CRM.Web
```

开发环境默认 API 地址为 `http://localhost:5268/api`，Swagger 位于 `http://localhost:5268/swagger`。

## 前端启动

```powershell
cd CRM.Client
npm install
npm run dev
```

如果后端端口变化，可在 `CRM.Client/.env.local` 中设置：

```text
VITE_API_BASE_URL=http://localhost:5268/api
```

## 演示流程

1. 新增客户。
2. 进入客户详情并新增联系人。
3. 进入合同管理并新增合同。
4. 选择客户后确认联系人下拉自动加载。
5. 添加合同明细，并用明细合计填充合同金额。
6. 保存合同并进入合同详情。
7. 启动合同。
8. 自动生成回款计划。
9. 登记实际回款。
10. 查看回款状态和合同状态变化。
11. 所有回款计划结清后，合同自动变为已完成。
12. 删除有关联合同的客户时执行逻辑删除，普通客户列表默认不显示。

## 当前已完成模块

- 客户管理
- 联系人管理
- 合同管理
- 回款计划管理
- 基础统计看板
- 基础数据枚举接口
- React 前端页面

## 当前限制

- 已接入 Cookie Authentication 轻量登录认证，并提供 Admin、Sales、EnterpriseUser、CustomerUser 四类角色。
- 权限通过 `[Authorize]` 与 `[Authorize(Roles = "...")]` 在 MVC 控制器、API 控制器和 Area 区域落地。
- Customer Portal 已完成最小只读入口，外部客户更完整的联系人、回款计划隔离页面仍可继续扩展。
- 基础数据暂以枚举方式实现，后续可扩展为数据字典表。
- 审批流程、电子签章、附件管理作为后续扩展。

## 多端登录与角色

本系统采用统一登录入口 `/Account/Login`，登录成功后根据用户角色进入不同端：

| 角色 | 端类型 | 登录后入口 | 权限边界 |
| --- | --- | --- | --- |
| `Admin` | 管理员端 | `/Admin/Dashboard/Index` | 管理全部用户、客户、联系人、合同、回款和基础数据 |
| `Sales` | 销售/企业业务端 | `/Sales/Dashboard/Index` | 管理自己负责的客户、联系人、合同和回款 |
| `EnterpriseUser` | 企业普通用户端 | `/Enterprise/Dashboard/Index` | 查看企业内部业务数据，不允许删除、取消合同或登记回款 |
| `CustomerUser` | 外部客户端 | `/CustomerPortal/Dashboard/Index` | 只读查看与绑定客户相关的合同信息 |

开发环境会初始化课程设计演示账号，密码使用 `PasswordHasher<AppUser>` 哈希保存：

- 管理员：`admin` / `Admin@123456`
- 销售：`sales` / `Sales@123456`
- 企业普通用户：`enterprise` / `Enterprise@123456`
- 外部客户用户：`customer` / `Customer@123456`

新增迁移：

```powershell
cd CRM.Lite
dotnet ef migrations add AddPortalRolesAndOwnershipFields -p CRM.Infrastructure -s CRM.Web
dotnet ef database update -p CRM.Infrastructure -s CRM.Web
```

更完整的校验、认证和多端设计见 `docs/validation-and-auth-design.md` 与 `docs/auth-and-portals.md`。
用户管理、权限矩阵和多用户登录验收方式见 `docs/auth-and-users.md`。

## 成员分工

领域建模师负责：

- 客户聚合建模
- 合同聚合建模
- 回款计划建模
- 合同状态流转
- 领域规则说明
- 领域模型文档维护
