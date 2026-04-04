using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutomationPortal.Domain.Entities;

namespace AutomationPortal.Infrastructure.Data.Configurations;

public sealed class GeminiKeyConfiguration : BaseEntityConfiguration<GeminiKey>
{
    public override void Configure(EntityTypeBuilder<GeminiKey> builder)
    {
        base.Configure(builder);

        builder.ToTable("gemini_keys");

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.KeyValue)
            .HasColumnName("key_value")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(g => g.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(g => g.Name)
            .IsUnique()
            .HasDatabaseName("ix_gemini_keys_name")
            .HasFilter("is_deleted = false");

        builder.HasIndex(g => g.UserId)
            .IsUnique()
            .HasDatabaseName("ix_gemini_keys_user_id")
            .HasFilter("is_deleted = false");
    }
}
