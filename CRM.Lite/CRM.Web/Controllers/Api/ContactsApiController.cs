using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers.Api;

[Route("api/contacts")]
[Authorize(Roles = "Admin,Sales,EnterpriseUser")]
public class ContactsApiController : ApiControllerBase
{
    private readonly IContactAppService _contactAppService;

    public ContactsApiController(IContactAppService contactAppService)
    {
        _contactAppService = contactAppService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Get(int id)
    {
        return OkResponse(await _contactAppService.GetAsync(id));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Sales")]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, UpdateContactDto input)
    {
        input.Id = id;
        await _contactAppService.UpdateAsync(input);
        return OkMessage("更新成功");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _contactAppService.DeleteAsync(id);
        return OkMessage("删除成功");
    }
}
