using CRM.Application.Contracts.Contacts;
using CRM.Application.Contracts.Contacts.Dtos;
using CRM.Application.Contracts.Customers;
using CRM.Application.Contracts.Customers.Dtos;
using CRM.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Web.Controllers.Api;

[Route("api/customers")]
public class CustomersApiController : ApiControllerBase
{
    private readonly ICustomerAppService _customerAppService;
    private readonly IContactAppService _contactAppService;

    public CustomersApiController(ICustomerAppService customerAppService, IContactAppService contactAppService)
    {
        _customerAppService = customerAppService;
        _contactAppService = contactAppService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetList([FromQuery] CustomerQueryDto query)
    {
        return OkResponse(await _customerAppService.GetListAsync(query));
    }

    [HttpGet("select-list")]
    public async Task<ActionResult<ApiResponse<List<CustomerSelectDto>>>> GetSelectList()
    {
        return OkResponse(await _customerAppService.GetSelectListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> Get(int id)
    {
        return OkResponse(await _customerAppService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create(CreateCustomerDto input)
    {
        await _customerAppService.CreateAsync(input);
        return OkMessage("创建成功");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, UpdateCustomerDto input)
    {
        input.Id = id;
        await _customerAppService.UpdateAsync(input);
        return OkMessage("更新成功");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _customerAppService.DeleteAsync(id);
        return OkMessage("删除成功");
    }

    [HttpGet("{customerId:int}/contacts")]
    public async Task<ActionResult<ApiResponse<List<ContactDto>>>> GetContacts(int customerId)
    {
        return OkResponse(await _contactAppService.GetListByCustomerIdAsync(customerId));
    }

    [HttpPost("{customerId:int}/contacts")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> CreateContact(int customerId, CreateContactDto input)
    {
        return OkResponse(await _contactAppService.CreateAsync(customerId, input), "创建成功");
    }
}
