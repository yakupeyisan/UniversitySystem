using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;

public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.ToTable("HealthRecords");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.Property(h => h.PersonId)
            .HasColumnName("PersonId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
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
        builder.Property(h => h.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(h => h.DeletedAt)
            .HasDatabaseName("IX_HealthRecords_DeletedAt");
        builder.Property(h => h.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(h => h.DeletedBy)
            .HasDatabaseName("IX_HealthRecords_DeletedBy");
        builder.Property(h => h.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(h => h.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(h => h.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(h => h.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasOne<Person>()
            .WithOne(p => p.HealthRecord)
            .HasForeignKey<HealthRecord>(h => h.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(h => new { h.LastCheckupDate, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_LastCheckupDate_IsDeleted");
        builder.HasIndex(h => new { h.CreatedAt, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_CreatedAt_IsDeleted");
        builder.HasIndex(h => new { h.DeletedAt, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_DeletedAt_IsDeleted");
        builder.HasIndex(h => new { h.PersonId, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_PersonId_IsDeleted");
    }
}