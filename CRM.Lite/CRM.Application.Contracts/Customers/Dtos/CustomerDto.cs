using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.Contracts.Customers.Dtos;

public class CustomerDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CreditCode { get; set; }
    public string? Industry { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? DetailAddress { get; set; }
    public string? Remark { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreationTime { get; set; }
}
