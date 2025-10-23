using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// HealthRecord Entity Configuration
/// </summary>
public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    /// <summary>
    /// Configure HealthRecord entity
    /// </summary>
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.ToTable("HealthRecords");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.PersonId)
            .IsRequired();

        builder.Property(h => h.BloodType)
            .IsRequired(false)
            .HasMaxLength(5);

        builder.Property(h => h.Allergies)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(h => h.ChronicDiseases)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(h => h.Medications)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(h => h.EmergencyHealthInfo)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(h => h.Notes)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(h => h.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(h => h.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(h => h.IsDeleted)
            .HasDefaultValue(false);

        // ==================== INDEXES ====================

        builder.HasIndex(h => h.PersonId)
            .IsUnique()
            .HasDatabaseName("IX_HealthRecords_PersonId_Unique");

        builder.HasIndex(h => h.IsDeleted)
            .HasDatabaseName("IX_HealthRecords_IsDeleted");
    }
}