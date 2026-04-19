using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence.Configurations;

public class CategoryGroupConfiguration : IEntityTypeConfiguration<CategoryGroup>
{
    public void Configure(EntityTypeBuilder<CategoryGroup> builder)
    {
        builder.ToTable("category_groups");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasColumnName("id");

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(g => g.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(g => g.IsSystem)
            .HasColumnName("is_system")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasMany(g => g.Categories)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(g => g.Categories)
            .HasField("_categories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(new
        {
            Id = WellKnownIds.InflowGroupId,
            Name = "Inflow",
            SortOrder = 0,
            IsSystem = true
        });
    }
}
