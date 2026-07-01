# CRM Lite 多端登录设计

## 统一登录入口

本系统保持 ASP.NET Core MVC + Razor Views + Bootstrap + jQuery 架构，不改造成前后端分离。所有用户统一访问 `/Account/Login` 登录，系统根据 `AppUser.Role` 自动分流到对应业务端。

## 端划分

| 角色 | 端类型 | 入口地址 | 主要权限 |
| --- | --- | --- | --- |
| `Admin` | 管理员端 Admin Portal | `/Admin/Dashboard/Index` | 管理全部用户、角色、客户、联系人、合同、回款和基础数据 |
| `Sales` | 销售/企业业务端 Sales Portal | `/Sales/Dashboard/Index` | 维护自己负责的客户、联系人、合同，登记回款，查看到期提醒 |
| `EnterpriseUser` | 企业普通用户端 Enterprise Portal | `/Enterprise/Dashboard/Index` | 查看客户、合同和回款信息，不允许删除、取消合同或登记回款 |
| `CustomerUser` | 外部客户端 Customer Portal | `/CustomerPortal/Dashboard/Index` | 只查看与自身企业相关的合同信息 |

`CustomerPortal` 当前为基础实现：已提供门户首页、合同列表和合同详情，只允许读取 `RelatedCustomerId` 绑定客户的合同数据。联系人、回款计划独立页面后续可继续补充。

## 登录分流

登录逻辑位于 `CRM.Web/Controllers/AccountController.cs`。登录成功后写入 Cookie，并根据角色执行跳转：

- `Admin` -> `/Admin/Dashboard/Index`
- `Sales` -> `/Sales/Dashboard/Index`
- `EnterpriseUser` -> `/Enterprise/Dashboard/Index`
- `CustomerUser` -> `/CustomerPortal/Dashboard/Index`

Claims 包含用户 ID、用户名、显示名称、角色、关联客户 ID 和关联销售 ID。未登录访问业务页面会跳转 `/Account/Login`，无权限访问会跳转 `/Account/AccessDenied`。

## 权限边界

- 后台客户、联系人、合同控制器不允许 `CustomerUser` 访问。
- 客户、联系人、合同的新增和编辑仅允许 `Admin`、`Sales`。
- 合同取消和删除仅允许 `Admin`。
- 回款登记允许 `Admin`、`Sales`。
- `EnterpriseUser` 只保留查看权限。
- `Sales` 在主要客户和合同列表中按 `OwnerUserId` 过滤自己负责的数据。
- `CustomerUser` 通过 `RelatedCustomerId` 过滤客户门户数据，未绑定客户时显示友好提示。

## 开发环境测试账号

开发环境启动时会初始化以下课程设计演示账号；如果账号已存在，不会重复创建：

- `admin` / `Admin@123456` / `Admin`
- `sales` / `Sales@123456` / `Sales`
- `enterprise` / `Enterprise@123456` / `EnterpriseUser`
- `customer` / `Customer@123456` / `CustomerUser`

密码使用 `PasswordHasher<AppUser>` 哈希保存，不保存明文密码。

## 运行与验收

```powershell
cd CRM.Lite
dotnet restore
dotnet build
dotnet ef database update -p CRM.Infrastructure -s CRM.Web
dotnet run --project CRM.Web
```

验收方式：

1. 访问 `/Account/Login`，确认显示统一登录入口。
2. 使用 `admin` 登录，确认进入 `/Admin/Dashboard/Index`。
3. 使用 `sales` 登录，确认进入 `/Sales/Dashboard/Index`，且不能访问 Admin Area。
4. 使用 `enterprise` 登录，确认进入 `/Enterprise/Dashboard/Index`，敏感操作无权限。
5. 使用 `customer` 登录，确认进入 `/CustomerPortal/Dashboard/Index`，不能访问后台 `/Contract/Index`。
6. 未登录访问 `/Customer/Index`、`/Contact/Index`、`/Contract/Index`，确认跳转登录页。
7. 无权限跨端访问时确认进入 `/Account/AccessDenied`。

## 数据库变更

多端登录补充迁移为 `AddPortalRolesAndOwnershipFields`，主要变更：

- `AppUsers` 增加 `RelatedCustomerId`、`RelatedSalesUserId`。
- `AppUsers.UpdatedAt` 调整为可空。
- `Customers` 增加 `OwnerUserId`。
- `Contracts` 增加 `OwnerUserId`。
