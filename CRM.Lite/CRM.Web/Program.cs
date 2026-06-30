using CRM.Application.Contacts;
using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contracts;
using CRM.Application.Contracts.Customers;
using CRM.Application.Customers;
using CRM.Domain.Repositories;
using CRM.Domain.Shared.Exceptions;
using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Repositories;
using CRM.Web.Models;
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

            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
