using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Customers.Dtos;

public class UpdateCustomerDto : CreateCustomerDto
{
    [Required]
    public int Id { get; set; }
}
