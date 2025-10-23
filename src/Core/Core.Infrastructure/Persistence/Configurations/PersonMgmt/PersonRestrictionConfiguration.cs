using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// PersonRestriction Entity Configuration
/// </summary>
public class PersonRestrictionConfiguration : IEntityTypeConfiguration<PersonRestriction>
{
    /// <summary>
    /// Configure PersonRestriction entity
    /// </summary>
    public void Configure(EntityTypeBuilder<PersonRestriction> builder)
    {
        builder.ToTable("PersonRestrictions");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PersonId)
            .IsRequired();

        builder.Property(r => r.RestrictionType)
            .IsRequired();

        builder.Property(r => r.RestrictionLevel)
            .IsRequired();

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.EndDate)
            .IsRequired(false);

        builder.Property(r => r.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Severity)
            .IsRequired();

        builder.Property(r => r.AppliedBy)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.IsDeleted)
            .HasDefaultValue(false);

        // ==================== INDEXES ====================

        builder.HasIndex(r => r.PersonId)
            .HasDatabaseName("IX_PersonRestrictions_PersonId");

        builder.HasIndex(r => r.RestrictionType)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionType");

        builder.HasIndex(r => r.RestrictionLevel)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionLevel");

        builder.HasIndex(r => r.IsDeleted)
            .HasDatabaseName("IX_PersonRestrictions_IsDeleted");

        builder.HasIndex(r => new { r.PersonId, r.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_PersonId_IsDeleted");
    }
}