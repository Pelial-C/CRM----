# 第5次开发 - Web层（CRM.Web）

**日期**：2026-06-27
**开发层**：CRM.Web（Web层 - Controller + Razor视图）
**涉及模块**：客户模块 + 联系人模块
**视图说明**：仅做简单测试用，前端由其他同学负责美化

---

## 修改文件

### 1. `CRM.Web/Controllers/CustomerController.cs`

**改动：** 完全重写，对接新的 ICustomerAppService 接口。

| Action | HTTP方法 | 说明 |
|--------|---------|------|
| Index(name?, industry?, pageIndex, pageSize) | GET | 客户列表页，支持搜索+分页 |
| Detail(id) | GET | 客户详情页，含联系人+合同列表 |
| Create() | GET | 新增客户表单页 |
| Create(CreateCustomerDto) | POST | 提交新增，失败时显示错误信息 |
| Edit(id) | GET | 编辑客户表单页 |
| Edit(UpdateCustomerDto) | POST | 提交编辑，失败时显示错误信息 |
| Delete(id) | POST | 删除客户（含二次确认） |

**对比原版变化：**
- 原 GetList/Edit(Create)/Delete 返回 JSON API → 改为服务端渲染页面+重定向
- 新增 Detail 页面
- 使用 try-catch 捕获 BusinessException 显示到页面

### 2. `CRM.Web/Views/Shared/_Layout.cshtml`

**改动：** 导航栏新增"客户管理"链接，"Home"改为"首页"。

---

## 新建文件

### 3. `CRM.Web/Controllers/ContactController.cs`

联系人 Controller，所有操作都需携带 customerId 参数。

| Action | HTTP方法 | 说明 |
|--------|---------|------|
| List(customerId, name?, title?) | GET | 某客户下联系人列表 |
| Create(customerId) | GET | 新增联系人表单页 |
| Create(CreateContactDto) | POST | 提交新增 |
| Edit(id) | GET | 编辑联系人表单页 |
| Edit(UpdateContactDto, customerId) | POST | 提交编辑 |
| Delete(id, customerId) | POST | 删除联系人 |

### 4. `CRM.Web/Views/Customer/Index.cshtml`

客户列表页，包含：
- 搜索栏（企业名称、行业）
- 客户表格（Id、名称、信用代码、行业、地区、是否删除、创建时间、操作按钮）
- 分页导航（上一页/下一页）

### 5. `CRM.Web/Views/Customer/Create.cshtml`

新增客户表单，使用 Tag Helper 绑定 CreateCustomerDto，含验证提示。

### 6. `CRM.Web/Views/Customer/Edit.cshtml`

编辑客户表单，使用 Tag Helper 绑定 UpdateCustomerDto，Id 用 hidden 传递。

### 7. `CRM.Web/Views/Customer/Detail.cshtml`

客户详情页，包含三个区域：
- 客户基本信息（dl 列表）
- 联系人列表（表格 + 新增/编辑/删除按钮）
- 合同列表（预留位置，已有数据时展示表格）

### 8. `CRM.Web/Views/Contact/List.cshtml`

联系人列表页，独立页面展示某客户下的联系人。

### 9. `CRM.Web/Views/Contact/Create.cshtml`

新增联系人表单，CustomerId 用 hidden 传递。

### 10. `CRM.Web/Views/Contact/Edit.cshtml`

编辑联系人表单，不含 CustomerId（联系人不能转移客户）。
