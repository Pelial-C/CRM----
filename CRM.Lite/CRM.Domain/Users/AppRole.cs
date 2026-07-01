using CRM.Domain.Shared.Enums;

namespace CRM.Domain.Users;

public class AppRole : AggregateRoot<int>
{
    public UserRole Role { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    protected AppRole()
    {
    }

    public AppRole(UserRole role, string name, string description)
    {
        Role = role;
        Name = name;
        Description = description;
    }
}
