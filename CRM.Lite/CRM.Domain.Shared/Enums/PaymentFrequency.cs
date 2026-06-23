using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Domain.Shared.Enums;

public enum PaymentFrequency
{
    Monthly = 1,     // 按月
    Quarterly = 3,   // 按季度
    HalfYearly = 6,  // 按半年
    Yearly = 12      // 按年
}
