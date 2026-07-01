# CRM Lite 数据校验与登录权限设计

## 1. 系统端划分

本系统是企业客户合同管理系统，采用统一登录入口，根据用户角色进入不同业务端。系统端划分为：

1. 登录端  
   所有用户统一访问 `/Account/Login` 登录，登录成功后根据角色进入不同功能范围。

2. 管理员端  
   管理员角色为 `Admin`，可访问 `/Admin/Dashboard/Index`，并拥有用户、角色、全部客户、全部合同、全部回款和基础数据维护权限。

3. 销售端  
   销售角色为 `Sales`，可访问 `/Sales/Dashboard/Index`，可维护自己负责的客户、联系人、合同，登记回款，并查看合同到期提醒。

4. 企业普通用户端  
   企业普通用户角色为 `EnterpriseUser`，可访问 `/Enterprise/Dashboard/Index`，用于企业内部普通人员查看客户、合同和回款计划，不允许删除、取消合同或登记回款。

5. 外部客户端  
   外部客户角色为 `CustomerUser`，可访问 `/CustomerPortal/Dashboard/Index`，当前已提供基础门户首页、合同列表和合同详情，只允许查看 `RelatedCustomerId` 绑定客户的数据。

## 2. DataAnnotations 覆盖范围

已补充或增强数据注解的 DTO：

- `CreateCustomerDto`
- `UpdateCustomerDto`
- `CreateContactDto`
- `UpdateContactDto`
- `CreateContractDto`
- `UpdateContractDto`
- `ContractItemDto`
- `AddPaymentPlanDto`
- `RecordPaymentDto`
- `CancelContractDto`
- `ContractListQueryDto`
- `GetContractListDto`

校验类型包括：

- 必填校验：客户名称、联系人姓名、合同编号、合同名称、合同金额、客户 ID、日期、回款金额等。
- 长度校验：合同编号 50、合同名称 100、客户名称 100、手机号 30、邮箱 100、备注 500 等。
- 金额范围校验：合同金额、明细单价、计划回款金额、实际回款金额。
- 日期范围校验：开始日期、结束日期、签订日期、计划日期、实际回款日期。
- 邮箱格式校验：联系人邮箱。
- 手机号格式校验：中国大陆手机号。
- 跨字段校验：结束日期不能早于开始日期、签订日期不能晚于结束日期、回款计划总额不能超过合同总金额、合同明细合计与合同总金额允许 0.01 元以内误差。
- 查询参数校验：关键字长度、页码、每页条数、日期区间。
- Razor 前端校验：创建和编辑页面引入 `_ValidationScriptsPartial`，合同页面补充 jQuery 表单提交前检查。
- 应用服务兜底校验：重复客户、重复信用代码、联系人重复、合同编号重复、合同金额与日期、回款计划和登记回款规则。

## 3. 登录认证说明

项目使用 Cookie Authentication 实现课程设计级轻量登录系统：

- 用户表为 `AppUsers`，角色表为 `AppRoles`。
- 角色枚举为 `UserRole`：`Admin`、`Sales`、`EnterpriseUser`、`CustomerUser`。
- 密码使用 `PasswordHasher<AppUser>` 保存哈希，不保存明文密码。
- 登录成功后写入 Cookie。
- Claims 包含：
  - `NameIdentifier`
  - `Name`
  - `Role`
  - `DisplayName`
  - `RelatedCustomerId`
  - `RelatedSalesUserId`
- 未登录访问业务页面跳转到 `/Account/Login`。
- 无权限访问跳转到 `/Account/AccessDenied`。
- MVC 控制器使用 `[Authorize]` 和 `[Authorize(Roles = "...")]` 控制访问边界。

开发环境初始化账号：

- `admin` / `Admin@123456` / `Admin`
- `sales` / `Sales@123456` / `Sales`
- `enterprise` / `Enterprise@123456` / `EnterpriseUser`
- `customer` / `Customer@123456` / `CustomerUser`

这些账号仅用于课程设计演示，代码中限制在开发环境初始化。

## 4. 页面与权限验收

可访问页面：

- `/Account/Login`
- `/Account/AccessDenied`
- `/Admin/Dashboard`
- `/Sales/Dashboard`
- `/Enterprise/Dashboard`
- `/CustomerPortal/Dashboard`
- `/CustomerPortal/Contracts`
- `/Customer/Index`
- `/Customer/Create`
- `/Customer/Edit/{id}`
- `/Contact/Index`
- `/Contact/Create`
- `/Contact/Edit/{id}`
- `/Contract/Index`
- `/Contract/Create`
- `/Contract/Edit/{id}`
- `/Contract/Detail/{id}`

验收步骤：

1. 执行 `dotnet restore`。
2. 执行 `dotnet build`。
3. 执行数据库迁移和更新。
4. 运行 `dotnet run --project CRM.Web`。
5. 未登录访问 `/Customer/Index`、`/Contact/Index`、`/Contract/Index`，应跳转登录页。
6. 使用 `admin` 登录，验证管理员首页和全部业务页面可访问。
7. 使用 `sales` 登录，验证客户、联系人、合同、回款维护可访问。
8. 使用 `enterprise` 登录，验证只能查看，新增、编辑、删除、作废、登记回款不可用或无权限。
9. 使用 `customer` 登录，验证进入客户门户，不能访问后台客户、联系人、合同控制器。
10. 合同创建时测试空合同编号、空合同名称、金额为 0、结束日期早于开始日期、预警天数越界。
11. 客户创建时测试客户名称为空、重复客户名称、重复统一社会信用代码。
12. 联系人创建时测试姓名为空、手机号格式错误、邮箱格式错误。
13. 退出登录后再次访问业务页面，应跳转登录页。

## 5. 数据库迁移

本次新增用户与角色实体，需要生成迁移：

```powershell
cd CRM.Lite
dotnet ef migrations add AddAuthUsersAndValidationEnhancements -p CRM.Infrastructure -s CRM.Web
dotnet ef database update -p CRM.Infrastructure -s CRM.Web
```

多端登录和数据归属字段补充迁移：

```powershell
cd CRM.Lite
dotnet ef migrations add AddPortalRolesAndOwnershipFields -p CRM.Infrastructure -s CRM.Web
dotnet ef database update -p CRM.Infrastructure -s CRM.Web
```

如果本机 `dotnet ef` 未安装，可先执行：

```powershell
dotnet tool install --global dotnet-ef
```
