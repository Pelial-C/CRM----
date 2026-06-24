using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Domain.ValueObjects;

public class Address
{
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? Detail { get; private set; }

    // EF Core 实例化需要
    protected Address() { }

    public Address(string province, string city, string district, string detail)
    {
        Province = province;
        City = city;
        District = district;
        Detail = detail;
    }
}
