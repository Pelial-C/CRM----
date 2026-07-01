using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Contracts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Areas.CustomerPortal.Controllers;

[Area("CustomerPortal")]
[Authorize(Roles = "CustomerUser")]
public class ContractsController : Controller
{
    private readonly IContractAppService _contractAppService;

    public ContractsController(IContractAppService contractAppService)
    {
        _contractAppService = contractAppService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var customerId = GetRelatedCustomerId();
        if (!customerId.HasValue)
        {
            ViewBag.UnboundMessage = "当前账号未绑定客户信息，请联系系统管理员。";
            return View(new List<ContractDto>());
        }

        var result = await _contractAppService.GetListAsync(new ContractListQueryDto
        {
            CustomerId = customerId.Value,
            PageIndex = 1,
            PageSize = 100
        });

        return View(result.Items);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var customerId = GetRelatedCustomerId();
        if (!customerId.HasValue)
        {
            ViewBag.UnboundMessage = "当前账号未绑定客户信息，请联系系统管理员。";
            return View(null);
        }

        var contract = await _contractAppService.GetAsync(id);
        if (contract.CustomerId != customerId.Value)
        {
            return Forbid();
        }

        return View(contract);
    }

    private int? GetRelatedCustomerId()
    {
        var value = User.FindFirst("RelatedCustomerId")?.Value;
        return int.TryParse(value, out var customerId) && customerId > 0 ? customerId : null;
    }
}
