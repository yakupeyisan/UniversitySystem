using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;
public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.ToTable("HealthRecords", "PersonMgmt");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.Property(h => h.BloodType)
            .HasColumnName("BloodType")
            .HasColumnType("nvarchar(10)")
            .HasMaxLength(10)
            .IsRequired(false);
        builder.HasIndex(h => h.BloodType)
            .HasDatabaseName("IX_HealthRecords_BloodType");
        builder.Property(h => h.Allergies)
            .HasColumnName("Allergies")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
        builder.Property(h => h.ChronicDiseases)
            .HasColumnName("ChronicDiseases")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
        builder.Property(h => h.Medications)
            .HasColumnName("Medications")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
        builder.Property(h => h.EmergencyHealthInfo)
            .HasColumnName("EmergencyHealthInfo")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
        builder.Property(h => h.Notes)
            .HasColumnName("Notes")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
        builder.Property(h => h.LastCheckupDate)
            .HasColumnName("LastCheckupDate")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(h => h.LastCheckupDate)
            .HasDatabaseName("IX_HealthRecords_LastCheckupDate");
        builder.Property(h => h.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(h => h.IsDeleted)
            .HasDatabaseName("IX_HealthRecords_IsDeleted");
        builder.Property(h => h.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();
        builder.Property(h => h.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(h => new { h.LastCheckupDate, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_LastCheckupDate_IsDeleted");
        builder.HasIndex(h => new { h.CreatedAt, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_CreatedAt_IsDeleted");
    }
}