using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Persistence;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractItem> ContractItems => Set<ContractItem>();
    public DbSet<PaymentPlan> PaymentPlans => Set<PaymentPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCustomer(modelBuilder);
        ConfigureContact(modelBuilder);
        ConfigureContract(modelBuilder);
        ConfigureContractItem(modelBuilder);
        ConfigurePaymentPlan(modelBuilder);
    }

    private static void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Customer>();

        entity.ToTable("Customers");
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Id).ValueGeneratedOnAdd();
        entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        entity.HasIndex(c => c.Name).IsUnique();
        entity.Property(c => c.CreditCode).HasMaxLength(50);
        entity.HasIndex(c => c.CreditCode).IsUnique().HasFilter("[CreditCode] IS NOT NULL");
        entity.Property(c => c.Industry).HasMaxLength(50);
        entity.Property(c => c.Remark).HasMaxLength(500);
        entity.Property(c => c.IsDeleted).HasDefaultValue(false);
        entity.Property(c => c.CreationTime).IsRequired();

        entity.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Province).HasMaxLength(50).HasColumnName("Province");
            address.Property(a => a.City).HasMaxLength(50).HasColumnName("City");
            address.Property(a => a.District).HasMaxLength(50).HasColumnName("District");
            address.Property(a => a.Detail).HasMaxLength(200).HasColumnName("DetailAddress");
        });

        entity.Navigation(c => c.Address).IsRequired();
        entity.Navigation(c => c.Contacts).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();

        entity.HasMany(c => c.Contacts)
            .WithOne()
            .HasForeignKey(c => c.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureContact(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Contact>();

        entity.ToTable("Contacts");
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Id).ValueGeneratedOnAdd();
        entity.Property(c => c.CustomerId).IsRequired();
        entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
        entity.Property(c => c.Title).HasMaxLength(50);
        entity.Property(c => c.Phone).HasMaxLength(30);
        entity.Property(c => c.Email).HasMaxLength(100);
    }

    private static void ConfigureContract(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Contract>();

        entity.ToTable("Contracts");
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Id).ValueGeneratedOnAdd();
        entity.Property(c => c.ContractNo).IsRequired().HasMaxLength(50);
        entity.HasIndex(c => c.ContractNo).IsUnique();
        entity.Property(c => c.ContractName).IsRequired().HasMaxLength(100);
        entity.Property(c => c.CabinetNo).HasMaxLength(50);
        entity.Property(c => c.CustomerId).IsRequired();
        entity.Property(c => c.CustomerName).IsRequired().HasMaxLength(100);
        entity.Ignore(c => c.PartyAName);
        entity.Property(c => c.ContactId);
        entity.Property(c => c.ContactName).HasMaxLength(50);
        entity.Property(c => c.RegionalCompany).HasMaxLength(100);
        entity.Property(c => c.AffiliatedCompany).HasMaxLength(100);
        entity.Property(c => c.TotalAmount).HasColumnType("decimal(18,2)");
        entity.Property(c => c.Status).HasConversion<int>().IsRequired();
        entity.Property(c => c.ServiceType).HasConversion<int>().IsRequired();
        entity.Property(c => c.ContractType).HasConversion<int>().IsRequired();
        entity.Property(c => c.PaymentFrequency).HasConversion<int>().IsRequired();
        entity.Property(c => c.Remark).HasMaxLength(500);
        entity.Property(c => c.CreationTime).IsRequired();

        entity.Navigation(c => c.Items).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();
        entity.Navigation(c => c.PaymentPlans).UsePropertyAccessMode(PropertyAccessMode.Field).AutoInclude();

        entity.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne<Contact>()
            .WithMany()
            .HasForeignKey(c => c.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.ContractId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(c => c.PaymentPlans)
            .WithOne()
            .HasForeignKey(p => p.ContractId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureContractItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ContractItem>();

        entity.ToTable("ContractItems");
        entity.HasKey(i => i.Id);
        entity.Property(i => i.Id).ValueGeneratedOnAdd();
        entity.Property(i => i.ContractId).IsRequired();
        entity.Property(i => i.ProductName).IsRequired().HasMaxLength(100);
        entity.Property(i => i.Quantity).IsRequired();
        entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        entity.Property(i => i.Subtotal).HasColumnType("decimal(18,2)");
    }

    private static void ConfigurePaymentPlan(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PaymentPlan>();

        entity.ToTable("PaymentPlans");
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Id).ValueGeneratedOnAdd();
        entity.Property(p => p.ContractId).IsRequired();
        entity.Property(p => p.PlanDate).IsRequired();
        entity.Property(p => p.PlanAmount).HasColumnType("decimal(18,2)");
        entity.Property(p => p.ActualAmount).HasColumnType("decimal(18,2)");
        entity.Property(p => p.Status).HasConversion<int>().IsRequired();
        entity.Property(p => p.Description).HasMaxLength(200);
    }
}
