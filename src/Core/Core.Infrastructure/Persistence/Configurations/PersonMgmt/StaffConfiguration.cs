using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// Staff Entity Configuration - EF Core Mapping
/// 
/// Sorumluluğu:
/// - Staff entity'nin database mapping'ini yapılandır
/// - Personel spesifik property'lerin column mapping'ini tanımla
/// - Unique constraints ve indexes'i konfigüre et
/// - ValueObjects (Address, EmergencyContact) mapping'ini yapılandır
/// - Person ile One-to-One relationship (shared primary key)
/// 
/// Table: Staff
/// Primary Key: Id (Guid) - Aynı zamanda PersonId
/// Foreign Key: Person.Id
/// 
/// Önemli:
/// - Staff.Id = Person.Id (Shared Primary Key Pattern)
/// - Bu pattern One-to-One relationship'i garanti eder
/// - Address ve EmergencyContact ValueObjects'lerdir
/// </summary>
public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        // ==================== TABLE CONFIGURATION ====================

        builder.ToTable("Staff", "PersonMgmt");
        builder.HasKey(s => s.Id);

        // ==================== PRIMARY KEY & FOREIGN KEY ====================

        // Id = PersonId (Shared Primary Key Pattern)
        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // Foreign Key to Person (implicit through shared Id)
        builder.HasOne<Person>()
            .WithOne(p => p.Staff)
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // ==================== SCALAR PROPERTIES ====================

        // Personel Numarası (Unique)
        builder.Property(s => s.EmployeeNumber)
            .HasColumnName("EmployeeNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(s => s.EmployeeNumber)
            .IsUnique()
            .HasDatabaseName("IX_Staff_EmployeeNumber_Unique");

        // Akademik Ünvan (Enum)
        builder.Property(s => s.AcademicTitle)
            .HasColumnName("AcademicTitle")
            .HasConversion(
                v => (int)v,
                v => (AcademicTitle)v
            )
            .HasDefaultValue(AcademicTitle.Unknown)
            .IsRequired();

        builder.HasIndex(s => s.AcademicTitle)
            .HasDatabaseName("IX_Staff_AcademicTitle");

        // İşe Alma Tarihi
        builder.Property(s => s.HireDate)
            .HasColumnName("HireDate")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(s => s.HireDate)
            .HasDatabaseName("IX_Staff_HireDate");

        // İşten Ayrılma Tarihi
        builder.Property(s => s.TerminationDate)
            .HasColumnName("TerminationDate")
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(s => s.TerminationDate)
            .HasDatabaseName("IX_Staff_TerminationDate");

        // Aktif Durumu
        builder.Property(s => s.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IX_Staff_IsActive");

        // ==================== VALUE OBJECTS ====================

        // Address (Complex ValueObject)
        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("AddressStreet")
                .HasMaxLength(200)
                .IsRequired(false);

            address.Property(a => a.City)
                .HasColumnName("AddressCity")
                .HasMaxLength(50)
                .IsRequired(false);

            address.Property(a => a.PostalCode)
                .HasColumnName("AddressPostalCode")
                .HasMaxLength(20)
                .IsRequired(false);

            address.Property(a => a.Country)
                .HasColumnName("AddressCountry")
                .HasMaxLength(50)
                .IsRequired(false);

            address.Property(a => a.FullAddress)
                .HasColumnName("AddressFullAddress")
                .HasMaxLength(500)
                .IsRequired(false);

        });

        // EmergencyContact (Complex ValueObject)
        builder.OwnsOne(s => s.EmergencyContact, emergencyContact =>
        {
            emergencyContact.Property(ec => ec.FullName)
                .HasColumnName("EmergencyContactFullName")
                .HasMaxLength(100)
                .IsRequired(false);

            emergencyContact.Property(ec => ec.Relationship)
                .HasColumnName("EmergencyContactRelationship")
                .HasMaxLength(50)
                .IsRequired(false);

            emergencyContact.Property(ec => ec.PhoneNumber)
                .HasColumnName("EmergencyContactPhoneNumber")
                .HasMaxLength(20)
                .IsRequired(false);

        });

        // ==================== AUDIT FIELDS ====================

        // Soft Delete
        builder.Property(s => s.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(s => s.IsDeleted)
            .HasDatabaseName("IX_Staff_IsDeleted");

        // Oluşturulma Tarihi
        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        // Güncelleme Tarihi
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();

        // ==================== COMPOSITE INDEXES ====================

        // IsActive + IsDeleted (Active staff query)
        builder.HasIndex(s => new { s.IsActive, s.IsDeleted })
            .HasDatabaseName("IX_Staff_IsActive_IsDeleted");

        // AcademicTitle + IsActive (Staff by position)
        builder.HasIndex(s => new { s.AcademicTitle, s.IsActive })
            .HasDatabaseName("IX_Staff_AcademicTitle_IsActive");

        // HireDate + IsActive (Hire date filter)
        builder.HasIndex(s => new { s.HireDate, s.IsActive })
            .HasDatabaseName("IX_Staff_HireDate_IsActive");

        // TerminationDate + IsDeleted (Terminated staff)
        builder.HasIndex(s => new { s.TerminationDate, s.IsDeleted })
            .HasDatabaseName("IX_Staff_TerminationDate_IsDeleted");
    }
}