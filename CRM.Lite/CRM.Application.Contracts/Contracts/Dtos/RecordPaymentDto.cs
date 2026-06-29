using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class RecordPaymentDto
{
    public int ContractId { get; set; }
    public int PlanId { get; set; }
    public decimal ActualAmount { get; set; }
    public DateTime ActualDate { get; set; } = DateTime.Today;
}
