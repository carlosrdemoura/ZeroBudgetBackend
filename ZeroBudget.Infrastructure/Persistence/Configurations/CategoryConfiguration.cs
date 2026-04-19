using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.GroupId)
            .HasColumnName("group_id")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(c => c.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(c => c.IsSystem)
            .HasColumnName("is_system")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Navigation(c => c.BudgetEntries)
            .HasField("_budgetEntries")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(c => c.Transactions)
            .HasField("_transactions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(new
        {
            Id = WellKnownIds.InflowCategoryId,
            GroupId = WellKnownIds.InflowGroupId,
            Name = "Inflow: Ready To Assign",
            SortOrder = 0,
            IsSystem = true
        });
    }
}
