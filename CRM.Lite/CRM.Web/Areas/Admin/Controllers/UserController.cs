using CRM.Domain.Shared.Enums;
using CRM.Domain.Users;
using CRM.Infrastructure.Persistence;
using CRM.Web.Models.AdminUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly CrmDbContext _dbContext;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public UserController(CrmDbContext dbContext, PasswordHasher<AppUser> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _dbContext.AppUsers
            .AsNoTracking()
            .OrderBy(u => u.Role)
            .ThenBy(u => u.UserName)
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel input)
    {
        if (!ModelState.IsValid) return View(input);

        var userName = input.UserName?.Trim() ?? string.Empty;
        if (await _dbContext.AppUsers.AnyAsync(u => u.UserName == userName))
        {
            ModelState.AddModelError(nameof(input.UserName), "用户名已存在");
            return View(input);
        }

        var user = new AppUser(userName, input.DisplayName ?? string.Empty, input.Role, input.Email, input.Phone);
        user.SetRelatedCustomer(input.RelatedCustomerId);
        user.SetActive(input.IsActive);
        user.SetPasswordHash(_passwordHasher.HashPassword(user, input.Password ?? string.Empty));

        _dbContext.AppUsers.Add(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _dbContext.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        return View(new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            RelatedCustomerId = user.RelatedCustomerId,
            IsActive = user.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel input)
    {
        if (!ModelState.IsValid) return View(input);

        var user = await _dbContext.AppUsers.FindAsync(input.Id);
        if (user == null) return NotFound();

        user.UpdateProfile(input.DisplayName ?? string.Empty, input.Email, input.Phone, input.Role, input.RelatedCustomerId);
        user.SetActive(input.IsActive);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(int id, bool isActive)
    {
        var user = await _dbContext.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        user.SetActive(isActive);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var user = await _dbContext.AppUsers.FindAsync(id);
        if (user == null) return NotFound();

        var newPassword = GetDefaultPasswordByRole(user.Role);
        user.SetPasswordHash(_passwordHasher.HashPassword(user, newPassword));
        await _dbContext.SaveChangesAsync();
        TempData["Message"] = $"已重置 {user.UserName} 的密码，请使用 README 中对应角色的演示密码。";
        return RedirectToAction(nameof(Index));
    }

    private static string GetDefaultPasswordByRole(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => "Admin@123456",
            UserRole.Sales => "Sales@123456",
            UserRole.EnterpriseUser => "Enterprise@123456",
            UserRole.CustomerUser => "Customer@123456",
            _ => "User@123456"
        };
    }
}
