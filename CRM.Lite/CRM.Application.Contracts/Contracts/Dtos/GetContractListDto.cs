using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class GetContractListDto
{
    public string? Keyword { get; set; }
    public int? Status { get; set; }
    public int? CustomerId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
