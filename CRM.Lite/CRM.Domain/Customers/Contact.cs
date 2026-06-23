using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Domain.Customers;

public class Contact : Entity<int>
{
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public string Title { get; private set; } // 职务
    public bool IsKeyDecisionMaker { get; private set; } // 是否关键决策人

    protected Contact() { }

    public Contact(string name, string phone, string title, bool isKeyDecisionMaker = false)
    {
        Name = name;
        Phone = phone;
        Title = title;
        IsKeyDecisionMaker = isKeyDecisionMaker;
    }
}
