using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Infrastructure.Persistence.Configurations;

public class BudgetEntryConfiguration : IEntityTypeConfiguration<BudgetEntry>
{
    public void Configure(EntityTypeBuilder<BudgetEntry> builder)
    {
        builder.ToTable("budget_entries");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        var yearMonthConverter = new ValueConverter<YearMonth, string>(
            ym => ym.ToString(),
            str => YearMonth.Parse(str));

        builder.Property(e => e.Month)
            .HasColumnName("month")
            .HasColumnType("varchar(7)")
            .HasConversion(yearMonthConverter)
            .IsRequired();

        builder.Property(e => e.Assigned)
            .HasColumnName("assigned")
            .HasColumnType("decimal(18,4)")
            .HasDefaultValue(0m)
            .IsRequired();

        builder.HasIndex(e => new { e.CategoryId, e.Month })
            .IsUnique();

        builder.HasOne(e => e.Category)
            .WithMany(c => c.BudgetEntries)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
