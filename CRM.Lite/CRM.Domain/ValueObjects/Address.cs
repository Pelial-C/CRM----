namespace CRM.Domain.ValueObjects;

public class Address
{
    public static Address Empty => new("", "", "", "");

    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? District { get; private set; }
    public string? Detail { get; private set; }

    protected Address() { }

    public Address(string? province, string? city, string? district, string? detail)
    {
        Province = string.IsNullOrWhiteSpace(province) ? null : province.Trim();
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        District = string.IsNullOrWhiteSpace(district) ? null : district.Trim();
        Detail = string.IsNullOrWhiteSpace(detail) ? null : detail.Trim();
    }
}
