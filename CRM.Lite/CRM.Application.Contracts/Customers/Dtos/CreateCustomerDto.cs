using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "企业名称不能为空")]
    [MaxLength(100, ErrorMessage = "企业名称不能超过100个字符")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "统一社会信用代码不能为空")]
    public string? CreditCode { get; set; }

    public string? Industry { get; set; }

    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? DetailAddress { get; set; }
}
