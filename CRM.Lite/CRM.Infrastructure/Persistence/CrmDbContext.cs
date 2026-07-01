using CRM.Domain.Contracts;
using CRM.Domain.Customers;
using CRM.Domain.Users;
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
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AppRole> AppRoles => Set<AppRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCustomer(modelBuilder);
        ConfigureContact(modelBuilder);
        ConfigureContract(modelBuilder);
        ConfigureContractItem(modelBuilder);
        ConfigurePaymentPlan(modelBuilder);
        ConfigureAppUser(modelBuilder);
        ConfigureAppRole(modelBuilder);
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
        entity.Property(c => c.OwnerUserId);
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
        entity.Property(c => c.OwnerUserId);
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

    private static void ConfigureAppUser(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AppUser>();

        entity.ToTable("AppUsers");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Id).ValueGeneratedOnAdd();
        entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
        entity.HasIndex(u => u.UserName).IsUnique();
        entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        entity.Property(u => u.DisplayName).IsRequired().HasMaxLength(50);
        entity.Property(u => u.Email).HasMaxLength(100);
        entity.HasIndex(u => u.Email);
        entity.Property(u => u.Phone).HasMaxLength(30);
        entity.Property(u => u.Role).HasConversion<int>().IsRequired();
        entity.Property(u => u.IsActive).HasDefaultValue(true);
        entity.Property(u => u.RelatedCustomerId);
        entity.Property(u => u.RelatedSalesUserId);
        entity.Property(u => u.CreatedAt).IsRequired();
        entity.Property(u => u.UpdatedAt);
        entity.Property(u => u.CreationTime).IsRequired();
    }

    private static void ConfigureAppRole(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AppRole>();

        entity.ToTable("AppRoles");
        entity.HasKey(r => r.Id);
        entity.Property(r => r.Id).ValueGeneratedOnAdd();
        entity.Property(r => r.Role).HasConversion<int>().IsRequired();
        entity.HasIndex(r => r.Role).IsUnique();
        entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
        entity.Property(r => r.Description).HasMaxLength(200);
        entity.Property(r => r.CreationTime).IsRequired();
    }
}
