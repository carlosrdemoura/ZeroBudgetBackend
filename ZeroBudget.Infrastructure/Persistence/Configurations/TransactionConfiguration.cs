using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(t => t.CategoryId)
            .HasColumnName("category_id")
            .IsRequired(false);

        builder.Property(t => t.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,4)")
            .IsRequired();

        builder.Property(t => t.Date)
            .HasColumnName("date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(t => t.Memo)
            .HasColumnName("memo")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(t => t.AffectsBudget)
            .HasColumnName("affects_budget")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.HasIndex(t => new { t.CategoryId, t.Date });
        builder.HasIndex(t => t.Date);
        builder.HasIndex(t => t.AccountId);

        builder.HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
