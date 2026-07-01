using System.Security.Claims;
using CRM.Domain.Users;
using CRM.Infrastructure.Persistence;
using CRM.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Domain.Shared.Enums;

namespace CRM.Web.Controllers;

public class AccountController : Controller
{
    private readonly CrmDbContext _dbContext;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public AccountController(CrmDbContext dbContext, PasswordHasher<AppUser> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userName = model.UserName?.Trim() ?? string.Empty;
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "用户名或密码错误");
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "该账号已被禁用，请联系管理员");
            return View(model);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password ?? string.Empty);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "用户名或密码错误");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("DisplayName", user.DisplayName)
        };
        if (user.RelatedCustomerId.HasValue)
        {
            claims.Add(new Claim("RelatedCustomerId", user.RelatedCustomerId.Value.ToString()));
        }

        if (user.RelatedSalesUserId.HasValue)
        {
            claims.Add(new Claim("RelatedSalesUserId", user.RelatedSalesUserId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(model.RememberMe ? 24 : 8)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return LocalRedirect(GetRedirectUrlByRole(user.Role));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectByRole(string? role)
    {
        return role switch
        {
            "Admin" => LocalRedirect(GetRedirectUrlByRole(UserRole.Admin)),
            "Sales" => LocalRedirect(GetRedirectUrlByRole(UserRole.Sales)),
            "EnterpriseUser" => LocalRedirect(GetRedirectUrlByRole(UserRole.EnterpriseUser)),
            "CustomerUser" => LocalRedirect(GetRedirectUrlByRole(UserRole.CustomerUser)),
            _ => RedirectToAction("Index", "Home")
        };
    }

    private static string GetRedirectUrlByRole(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "/Admin/Dashboard/Index",
            UserRole.Sales => "/Sales/Dashboard/Index",
            UserRole.EnterpriseUser => "/Enterprise/Dashboard/Index",
            UserRole.CustomerUser => "/CustomerPortal/Dashboard/Index",
            _ => "/Home/Index"
        };
    }
}
