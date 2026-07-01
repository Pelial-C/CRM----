using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Areas.CustomerPortal.Controllers;

[Area("CustomerPortal")]
[Authorize(Roles = "CustomerUser")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.RelatedCustomerId = User.FindFirst("RelatedCustomerId")?.Value;
        return View();
    }
}
