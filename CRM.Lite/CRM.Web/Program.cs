using CRM.Application.Contacts;
using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Customers;
using CRM.Application.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Enums;
using CRM.Domain.Shared.Exceptions;
using CRM.Domain.Users;
using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Repositories;
using CRM.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContractAppService = CRM.Application.Contracts.ContractAppService;

namespace CRM.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var message = context.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "参数校验失败" : e.ErrorMessage)
                            .FirstOrDefault() ?? "参数校验失败";

                        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(ApiResponse<object>.Fail(message));
                    };
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactClient", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<CrmDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
            builder.Services.AddScoped<ICustomerAppService, CustomerAppService>();
            builder.Services.AddScoped<IContactAppService, ContactAppService>();
            builder.Services.AddScoped<IContractAppService, ContractAppService>();
            builder.Services.AddScoped<PasswordHasher<AppUser>>();

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                SeedDevelopmentUsers(app);
            }

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (BusinessException ex) when (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(ex.Message));
                }
                catch (Exception ex) when (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail($"服务器异常：{ex.Message}"));
                }
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("ReactClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void SeedDevelopmentUsers(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<AppUser>>();

            dbContext.Database.Migrate();

            EnsureDevelopmentRole(dbContext, UserRole.Admin, "管理员", "管理用户、客户、合同、基础数据和全部回款信息");
            EnsureDevelopmentRole(dbContext, UserRole.Sales, "销售人员", "维护负责客户、联系人、合同和回款");
            EnsureDevelopmentRole(dbContext, UserRole.EnterpriseUser, "企业普通用户", "查看客户、合同和回款计划");
            EnsureDevelopmentRole(dbContext, UserRole.CustomerUser, "外部客户用户", "查看自身企业相关合同、回款计划和联系人");

            EnsureDevelopmentUser(dbContext, passwordHasher, "admin", "管理员", "admin@example.com", UserRole.Admin, "Admin@123456");
            EnsureDevelopmentUser(dbContext, passwordHasher, "sales", "销售人员", "sales@example.com", UserRole.Sales, "Sales@123456");
            EnsureDevelopmentUser(dbContext, passwordHasher, "enterprise", "企业普通用户", "enterprise@example.com", UserRole.EnterpriseUser, "Enterprise@123456");
            EnsureDevelopmentUser(dbContext, passwordHasher, "customer", "外部客户用户", "customer@example.com", UserRole.CustomerUser, "Customer@123456");
        }

        private static void EnsureDevelopmentUser(
            CrmDbContext dbContext,
            PasswordHasher<AppUser> passwordHasher,
            string userName,
            string displayName,
            string email,
            UserRole role,
            string password)
        {
            if (dbContext.AppUsers.Any(u => u.UserName == userName)) return;

            var user = new AppUser(userName, displayName, role, email);
            user.SetPasswordHash(passwordHasher.HashPassword(user, password));
            dbContext.AppUsers.Add(user);
            dbContext.SaveChanges();
        }

        private static void EnsureDevelopmentRole(CrmDbContext dbContext, UserRole role, string name, string description)
        {
            if (dbContext.AppRoles.Any(r => r.Role == role)) return;

            dbContext.AppRoles.Add(new AppRole(role, name, description));
            dbContext.SaveChanges();
        }
    }
}
