# CRM Lite 业务规则矩阵

## 客户规则

| 编号 | 规则描述 | 所属聚合 | 当前实现位置 | 测试覆盖情况 |
| --- | --- | --- | --- | --- |
| CUS-001 | 客户名称不能为空 | Customer | `Customer.UpdateInfo` | `CustomerDomainTests.Customer_name_cannot_be_empty` |
| CUS-002 | 客户删除采用逻辑删除，设置 `IsDeleted` | Customer | `Customer.MarkAsDeleted` | 由应用服务和历史测试覆盖，当前文档记录 |
| CUS-003 | 已删除客户不能新增联系人 | Customer | `Customer.AddContact` | `CustomerDomainTests.Deleted_customer_cannot_add_contact` |
| CUS-004 | 已删除客户不能维护联系人 | Customer | `Customer.RemoveContact` | 间接覆盖，后续可补充删除联系人测试 |
| CUS-005 | 同一客户下联系人姓名 + 手机号不能重复 | Customer | `Customer.AddContact` | `CustomerDomainTests.Contact_name_and_phone_cannot_duplicate_in_same_customer` |
| CUS-006 | 客户名称、统一社会信用代码不能重复 | Customer | `CustomerAppService` | 应用服务规则，后续可增加服务层测试 |

## 联系人规则

| 编号 | 规则描述 | 所属聚合 | 当前实现位置 | 测试覆盖情况 |
| --- | --- | --- | --- | --- |
| CON-001 | 联系人姓名不能为空 | Customer | `Contact.Update` | 由 DTO 校验和领域构造保护，后续可补充直接领域测试 |
| CON-002 | 联系人必须归属于有效客户 | Customer | `Contact` 构造函数、`ContactAppService` | 应用服务规则，后续可增加服务层测试 |
| CON-003 | 联系人可以标记为关键决策人，不能影响正常保存 | Customer | `Contact.Update` | 当前未单独断言，后续可补充测试 |
| CON-004 | 删除联系人前应检查是否被合同使用 | Customer / Contract | `ContactAppService` | 应用服务规则，后续可增加服务层测试 |

## 合同规则

| 编号 | 规则描述 | 所属聚合 | 当前实现位置 | 测试覆盖情况 |
| --- | --- | --- | --- | --- |
| CTR-001 | 合同编号不能为空 | Contract | `Contract.UpdateBasicInfo` | `ContractDomainTests.Contract_no_cannot_be_empty` |
| CTR-002 | 合同名称不能为空 | Contract | `Contract.UpdateBasicInfo` | DTO 与领域保护，后续可补充直接测试 |
| CTR-003 | 合同金额必须大于 0 | Contract | `Contract.UpdateBasicInfo` | `ContractDomainTests.Contract_amount_must_be_positive` |
| CTR-004 | 合同结束日期不能早于开始日期 | Contract | `Contract.UpdateBasicInfo` | `ContractDomainTests.Contract_end_date_cannot_be_before_start_date` |
| CTR-005 | 合同必须关联有效客户 | Contract | `Contract.UpdateBasicInfo`、`ContractAppService.GetValidCustomerAsync` | 应用服务规则，后续可增加服务层测试 |
| CTR-006 | 合同明细至少包含一项 | Contract | `Contract.ReplaceItems` | `ContractDomainTests.Contract_items_cannot_be_empty` |
| CTR-007 | 合同明细单价必须大于 0 | Contract | `ContractItem.Update` | `ContractDomainTests.Contract_item_unit_price_must_be_greater_than_zero` |
| CTR-008 | 合同明细金额总和必须等于合同总金额 | Contract | `Contract.ReplaceItems` | `ContractDomainTests.Contract_item_total_must_equal_contract_total_amount` |
| CTR-009 | 合同作废必须填写原因 | Contract | `Contract.Cancel` | `ContractDomainTests.Contract_cancel_reason_cannot_be_empty` |
| CTR-010 | 已取消合同不能登记回款 | Contract | `Contract.CanRecordPayment`、`Contract.RecordPayment` | `ContractDomainTests.Cancelled_contract_cannot_record_payment` |
| CTR-011 | 所有回款计划完成后合同自动完成 | Contract | `Contract.TryCompleteByPayments` | `PaymentPlanDomainTests.Contract_completes_when_all_payment_plans_are_paid` |

## 回款规则

| 编号 | 规则描述 | 所属聚合 | 当前实现位置 | 测试覆盖情况 |
| --- | --- | --- | --- | --- |
| PAY-001 | 计划回款金额必须大于 0 | Contract | `PaymentPlan` 构造函数、`Contract.AddPaymentPlan` | 领域保护，后续可补充直接测试 |
| PAY-002 | 回款计划累计金额不能超过合同总金额 | Contract | `Contract.EnsurePaymentPlanTotal` | 历史测试已覆盖，后续可保留服务层测试 |
| PAY-003 | 实际回款金额必须大于 0 | Contract | `PaymentPlan.RecordActualPayment` | DTO 与领域保护，后续可补充直接测试 |
| PAY-004 | 实际回款不能超过计划金额 | Contract | `PaymentPlan.RecordActualPayment` | `PaymentPlanDomainTests.Actual_payment_cannot_exceed_plan_amount` |
| PAY-005 | 未结清且计划日期早于当前日期的回款计划可标记为逾期 | Contract | `PaymentPlan.MarkOverdue` | `PaymentPlanDomainTests.Overdue_payment_plan_can_be_marked_overdue` |

## 用户权限规则

| 编号 | 规则描述 | 所属聚合 | 当前实现位置 | 测试覆盖情况 |
| --- | --- | --- | --- | --- |
| AUTH-001 | 系统用户用户名不能为空 | AppUser | `AppUser` 构造函数 | 后续可补充用户领域测试 |
| AUTH-002 | 密码必须保存哈希，不能为空 | AppUser | `AppUser.SetPasswordHash`、`Program.SeedDevelopmentUsers` | 后续可补充用户领域测试 |
| AUTH-003 | 停用账号不能登录 | AppUser | `AccountController.Login` | Web 层规则，后续可增加集成测试 |
| AUTH-004 | Sales 只能查看自己负责的客户和合同 | Customer / Contract / AppUser | `OwnerUserId`、MVC/API Controller 查询过滤 | 已人工验证，后续可增加应用服务测试 |
| AUTH-005 | CustomerUser 只能访问 CustomerPortal，并通过 `RelatedCustomerId` 查看绑定客户数据 | AppUser / Customer | `AccountController`、`CustomerPortal` Area | 已人工验证，后续可增加集成测试 |
