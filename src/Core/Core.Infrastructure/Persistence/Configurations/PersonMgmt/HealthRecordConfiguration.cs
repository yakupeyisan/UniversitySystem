using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// HealthRecord Entity Configuration - EF Core Mapping
/// 
/// Sorumluluğu:
/// - HealthRecord entity'nin database mapping'ini yapılandır
/// - Sağlık kaydı spesifik property'lerin column mapping'ini tanımla
/// - Tıbbi veriler için encryption/masking tavsiyelerini (comment) ekle
/// - Person ile One-to-One relationship'i konfigüre et
/// - Soft delete ve audit field'larını konfigüre et
/// 
/// Table: HealthRecords
/// Primary Key: Id (Guid)
/// Foreign Key: PersonId (Guid)
/// 
/// Önemli:
/// - HealthRecord.Id != Person.Id (independent primary key)
/// - HealthRecord'ların PersonId'ye ihtiyacı var
/// - Tıbbi veriler hassastır (GDPR/privacy compliance)
/// </summary>
public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        // ==================== TABLE CONFIGURATION ====================

        builder.ToTable("HealthRecords", "PersonMgmt");
        builder.HasKey(h => h.Id);

        // ==================== PRIMARY KEY ====================

        builder.Property(h => h.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // ==================== FOREIGN KEYS ====================

        // PersonId - One-to-One relationship
        // Note: Bu ilişki Person'dan gelecek şekilde navigasyon property üzerinden yapılır
        // HealthRecord'ın PersonId'ye ihtiyacı var

        // ==================== SCALAR PROPERTIES ====================

        // Kan Grubu
        builder.Property(h => h.BloodType)
            .HasColumnName("BloodType")
            .HasColumnType("nvarchar(10)")
            .HasMaxLength(10)
            .IsRequired(false);

        builder.HasIndex(h => h.BloodType)
            .HasDatabaseName("IX_HealthRecords_BloodType");

        // Alerji Bilgisi
        builder.Property(h => h.Allergies)
            .HasColumnName("Allergies")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Kronik Hastalıklar
        builder.Property(h => h.ChronicDiseases)
            .HasColumnName("ChronicDiseases")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Kullanılan İlaçlar
        builder.Property(h => h.Medications)
            .HasColumnName("Medications")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Acil Durum Sağlık Bilgisi
        builder.Property(h => h.EmergencyHealthInfo)
            .HasColumnName("EmergencyHealthInfo")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Notlar / Açıklamalar
        builder.Property(h => h.Notes)
            .HasColumnName("Notes")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Son Kontrol Tarihi
        builder.Property(h => h.LastCheckupDate)
            .HasColumnName("LastCheckupDate")
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(h => h.LastCheckupDate)
            .HasDatabaseName("IX_HealthRecords_LastCheckupDate");

        // ==================== AUDIT FIELDS ====================

        // Soft Delete
        builder.Property(h => h.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(h => h.IsDeleted)
            .HasDatabaseName("IX_HealthRecords_IsDeleted");

        // Oluşturulma Tarihi
        builder.Property(h => h.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        // Güncelleme Tarihi
        builder.Property(h => h.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();

        // ==================== COMPOSITE INDEXES ====================

        // LastCheckupDate + IsDeleted (Recent checkups)
        builder.HasIndex(h => new { h.LastCheckupDate, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_LastCheckupDate_IsDeleted");

        // CreatedAt + IsDeleted (Audit trail)
        builder.HasIndex(h => new { h.CreatedAt, h.IsDeleted })
            .HasDatabaseName("IX_HealthRecords_CreatedAt_IsDeleted");

        // ==================== COMMENTS & DOCUMENTATION ====================

        // Not: Tıbbi veriler hassastır
        // - GDPR uyumluluğu gerekli
        // - Encryption in-transit ve at-rest gerekli olabilir
        // - Access logging önemlidir
        // - Data retention policy uygulanmalı
    }
}