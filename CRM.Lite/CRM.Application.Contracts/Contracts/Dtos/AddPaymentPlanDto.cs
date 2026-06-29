using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class AddPaymentPlanDto
{
    public int ContractId { get; set; }
    public DateTime PlanDate { get; set; }
    public decimal PlanAmount { get; set; }
    public string? Description { get; set; }
}
