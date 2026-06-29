using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerAppService _customerAppService;

        // 依赖注入 AppService
        public CustomerController(ICustomerAppService customerAppService)
        {
            _customerAppService = customerAppService;
        }

        // ================= 1. 页面路由 (为明天前端准备) =================

        /// <summary>
        /// 客户列表页面
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增客户页面
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 编辑客户页面
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerAppService.GetByIdAsync(id);
            // 将 DTO 转换为 UpdateCustomerDto 传给视图 (明天前端用)
            var updateDto = new UpdateCustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                CreditCode = customer.CreditCode,
                Industry = customer.Industry,
                Province = customer.Province,
                City = customer.City,
                District = customer.District,
                DetailAddress = customer.DetailAddress
            };
            return View(updateDto);
        }


        // ================= 2. 数据 API (供前端 Ajax 调用) =================

        /// <summary>
        /// 获取客户列表数据 (JSON API)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList(string? keyword, bool includeDeleted = false)
        {
            var data = await _customerAppService.GetListAsync(new CustomerQueryDto
            {
                Keyword = keyword,
                IncludeDeleted = includeDeleted
            });
            // 返回统一的 JSON 格式，方便前端处理
            return Json(new { code = 200, msg = "success", data = data });
        }

        /// <summary>
        /// 提交新增客户 (JSON API)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDto input)
        {
            // ModelState 校验 (基于 DTO 上的 [Required] 等注解)
            if (!ModelState.IsValid)
            {
                return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
            }

            await _customerAppService.CreateAsync(input);
            return Json(new { code = 200, msg = "创建成功" });
        }

        /// <summary>
        /// 提交编辑客户 (JSON API)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] UpdateCustomerDto input)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { code = 400, msg = "参数校验失败", data = ModelState });
            }

            await _customerAppService.UpdateAsync(input);
            return Json(new { code = 200, msg = "更新成功" });
        }

        /// <summary>
        /// 删除客户 (JSON API)
        /// </summary>
        [HttpPost] // 删除操作建议用 POST 或 DELETE，不要用 GET 防止爬虫误删
        public async Task<IActionResult> Delete(int id)
        {
            await _customerAppService.DeleteAsync(id);
            return Json(new { code = 200, msg = "删除成功" });
        }
    }
}
