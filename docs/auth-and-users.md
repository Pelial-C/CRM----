# CRM Lite 多用户登录与用户管理说明

## 1. 登录设计

系统采用统一账号登录入口：

```text
/Account/Login
```

用户输入用户名和密码后，`AccountController` 使用 `PasswordHasher<AppUser>` 校验密码哈希，并根据 `AppUser.Role` 跳转到对应端。

| 角色 | 跳转地址 | 说明 |
| --- | --- | --- |
| `Admin` | `/Admin/Dashboard/Index` | 系统管理员，拥有最高权限 |
| `Sales` | `/Sales/Dashboard/Index` | 销售人员，负责客户、联系人、合同、回款 |
| `EnterpriseUser` | `/Enterprise/Dashboard/Index` | 企业普通用户，只能查看业务数据 |
| `CustomerUser` | `/CustomerPortal/Dashboard/Index` | 外部客户用户，只能查看与自己企业相关的数据 |

登录成功后写入 Cookie，Claims 包含：

- `NameIdentifier`
- `Name`
- `Role`
- `DisplayName`
- `RelatedCustomerId`，如果存在
- `RelatedSalesUserId`，如果存在

未登录访问业务页面会跳转 `/Account/Login`；无权限访问会跳转 `/Account/AccessDenied`。

## 2. 用户实体

用户实体位于 `CRM.Lite/CRM.Domain/Users/AppUser.cs`，主要字段：

- `UserName`
- `PasswordHash`
- `DisplayName`
- `Email`
- `Phone`
- `Role`
- `IsActive`
- `RelatedCustomerId`
- `CreatedAt`
- `UpdatedAt`

密码只保存哈希，不保存明文。`IsActive = false` 时禁止登录。`RelatedCustomerId` 用于 `CustomerUser` 绑定客户资料。

## 3. 权限矩阵

| 功能 | Admin | Sales | EnterpriseUser | CustomerUser |
| --- | --- | --- | --- | --- |
| 管理员首页 | 允许 | 禁止 | 禁止 | 禁止 |
| 用户管理 | 允许 | 禁止 | 禁止 | 禁止 |
| 客户查看 | 允许 | 允许 | 允许 | 禁止 |
| 客户新增/编辑 | 允许 | 允许 | 禁止 | 禁止 |
| 客户删除 | 允许 | 禁止 | 禁止 | 禁止 |
| 联系人查看 | 允许 | 允许 | 允许 | 禁止 |
| 联系人新增/编辑 | 允许 | 允许 | 禁止 | 禁止 |
| 联系人删除 | 允许 | 禁止 | 禁止 | 禁止 |
| 合同查看 | 允许 | 允许 | 允许 | 禁止 |
| 合同新增/编辑 | 允许 | 允许 | 禁止 | 禁止 |
| 合同作废/删除 | 允许 | 禁止 | 禁止 | 禁止 |
| 回款登记 | 允许 | 允许 | 禁止 | 禁止 |
| 客户门户 | 禁止 | 禁止 | 禁止 | 允许 |

`CustomerUser` 不访问后台 `CustomerController`、`ContactController`、`ContractController`，只访问 `CustomerPortal` Area。

## 4. 管理员用户管理

管理员用户管理页面：

```text
/Admin/User/Index
/Admin/User/Create
/Admin/User/Edit/{id}
```

已实现：

- 用户列表；
- 新增用户；
- 编辑显示名称、邮箱、手机号、角色、是否启用、关联客户 ID；
- 启用/禁用用户；
- 按角色重置演示密码。

## 5. 开发环境测试账号

开发环境启动时会初始化以下账号；如果账号已存在，不会重复创建：

- `admin` / `Admin@123456` / `Admin`
- `sales` / `Sales@123456` / `Sales`
- `enterprise` / `Enterprise@123456` / `EnterpriseUser`
- `customer` / `Customer@123456` / `CustomerUser`

这些账号仅用于课程设计演示。

## 6. 验证方式

1. 运行 `dotnet build`。
2. 执行 `dotnet ef database update -p CRM.Infrastructure -s CRM.Web`。
3. 运行 `dotnet run --project CRM.Web`。
4. 访问 `/Account/Login`。
5. 分别使用四类账号登录，确认跳转到对应 Dashboard。
6. 未登录访问 `/Contract/Index`，应跳转登录页。
7. 使用 `sales` 访问 `/Admin/Dashboard/Index`，应进入 AccessDenied。
8. 使用 `enterprise` 尝试删除或作废合同，应进入 AccessDenied。
9. 使用 `customer` 访问 `/Contract/Index`，应进入 AccessDenied。
10. 退出登录后再次访问业务页面，应重新跳转登录页。
