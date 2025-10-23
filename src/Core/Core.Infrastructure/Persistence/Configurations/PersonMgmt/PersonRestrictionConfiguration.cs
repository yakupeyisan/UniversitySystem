using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// PersonRestriction Entity Configuration - EF Core Mapping
/// 
/// Sorumluluğu:
/// - PersonRestriction entity'nin database mapping'ini yapılandır
/// - Kısıtlama spesifik property'lerin column mapping'ini tanımla
/// - Unique constraints ve indexes'i konfigüre et
/// - Person ile One-to-Many relationship'i konfigüre et
/// - Soft delete ve audit field'larını konfigüre et
/// 
/// Table: PersonRestrictions
/// Primary Key: Id (Guid)
/// Foreign Key: PersonId (Guid)
/// 
/// Önemli:
/// - PersonRestriction.Id != Person.Id (independent primary key)
/// - PersonRestriction'ların PersonId'ye ihtiyacı var
/// - Bir kişinin birden çok kısıtlaması olabilir (One-to-Many)
/// - StartDate ve EndDate'e göre aktif/pasif durumu kontrol edilir
/// </summary>
public class PersonRestrictionConfiguration : IEntityTypeConfiguration<PersonRestriction>
{
    public void Configure(EntityTypeBuilder<PersonRestriction> builder)
    {
        // ==================== TABLE CONFIGURATION ====================

        builder.ToTable("PersonRestrictions", "PersonMgmt");
        builder.HasKey(pr => pr.Id);

        // ==================== PRIMARY KEY ====================

        builder.Property(pr => pr.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // ==================== FOREIGN KEYS ====================

        // PersonId - One-to-Many relationship
        // Note: Bu property Entity class'ında bulunmaz ancak DbContext üzerinden
        // navigation property konfigüre edilir.
        // Konfigürasyon: Person HasMany Restrictions

        // AppliedBy (User/Admin who applied the restriction)
        builder.Property(pr => pr.AppliedBy)
            .HasColumnName("AppliedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasIndex(pr => pr.AppliedBy)
            .HasDatabaseName("IX_PersonRestrictions_AppliedBy");

        // ==================== SCALAR PROPERTIES ====================

        // Kısıtlama Türü (Enum)
        builder.Property(pr => pr.RestrictionType)
            .HasColumnName("RestrictionType")
            .HasConversion(
                v => (int)v,
                v => (RestrictionType)v
            )
            .IsRequired();

        builder.HasIndex(pr => pr.RestrictionType)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionType");

        // Kısıtlama Seviyesi (Enum)
        builder.Property(pr => pr.RestrictionLevel)
            .HasColumnName("RestrictionLevel")
            .HasConversion(
                v => (int)v,
                v => (RestrictionLevel)v
            )
            .IsRequired();

        builder.HasIndex(pr => pr.RestrictionLevel)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionLevel");

        // Başlama Tarihi
        builder.Property(pr => pr.StartDate)
            .HasColumnName("StartDate")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(pr => pr.StartDate)
            .HasDatabaseName("IX_PersonRestrictions_StartDate");

        // Bitiş Tarihi (null = kalıcı kısıtlama)
        builder.Property(pr => pr.EndDate)
            .HasColumnName("EndDate")
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(pr => pr.EndDate)
            .HasDatabaseName("IX_PersonRestrictions_EndDate");

        // Neden
        builder.Property(pr => pr.Reason)
            .HasColumnName("Reason")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired();

        // Ciddiyet Seviyesi (1-10 veya enum)
        builder.Property(pr => pr.Severity)
            .HasColumnName("Severity")
            .HasColumnType("int")
            .IsRequired();

        builder.HasIndex(pr => pr.Severity)
            .HasDatabaseName("IX_PersonRestrictions_Severity");

        // Aktif Durumu
        builder.Property(pr => pr.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(pr => pr.IsActive)
            .HasDatabaseName("IX_PersonRestrictions_IsActive");

        // ==================== AUDIT FIELDS ====================

        // Soft Delete
        builder.Property(pr => pr.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(pr => pr.IsDeleted)
            .HasDatabaseName("IX_PersonRestrictions_IsDeleted");

        // Oluşturulma Tarihi
        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        // Güncelleme Tarihi
        builder.Property(pr => pr.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();

        // ==================== COMPOSITE INDEXES ====================

        // RestrictionType + IsActive + IsDeleted (Active restrictions by type)
        builder.HasIndex(pr => new { pr.RestrictionType, pr.IsActive, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_Type_Active_Deleted");

        // StartDate + EndDate + IsActive (Date range queries)
        builder.HasIndex(pr => new { pr.StartDate, pr.EndDate, pr.IsActive })
            .HasDatabaseName("IX_PersonRestrictions_DateRange_Active");

        // AppliedBy + CreatedAt (Who applied restrictions)
        builder.HasIndex(pr => new { pr.AppliedBy, pr.CreatedAt })
            .HasDatabaseName("IX_PersonRestrictions_AppliedBy_CreatedAt");

        // IsActive + EndDate (About to expire restrictions)
        builder.HasIndex(pr => new { pr.IsActive, pr.EndDate })
            .HasDatabaseName("IX_PersonRestrictions_Active_EndDate");

        // Severity + RestrictionType (Critical restrictions)
        builder.HasIndex(pr => new { pr.Severity, pr.RestrictionType })
            .HasDatabaseName("IX_PersonRestrictions_Severity_Type");

        // CreatedAt + IsDeleted (Audit trail)
        builder.HasIndex(pr => new { pr.CreatedAt, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_CreatedAt_IsDeleted");

        // ==================== CONSTRAINTS & RULES ====================

        // Not: Validasyonlar
        // - StartDate < EndDate (if EndDate is not null)
        // - Severity: 1-10 range
        // - IsActive değeri StartDate/EndDate'e göre otomatik belirlenebilir
    }
}