using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// Person Entity Configuration - EF Core Mapping
/// 
/// Sorumluluğu:
/// - Person aggregate root'un database mapping'ini yapılandır
/// - Tüm property'lerin column mapping'ini tanımla
/// - Unique constraints ve indexes'i konfigüre et
/// - Soft delete ve audit field'larını konfigüre et
/// - One-to-One relationships: Student, Staff, HealthRecord
/// - One-to-Many relationship: Restrictions
/// 
/// Table: Persons
/// Primary Key: Id (Guid)
/// </summary>
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        // ==================== TABLE CONFIGURATION ====================

        builder.ToTable("Persons", "PersonMgmt");
        builder.HasKey(p => p.Id);

        // ==================== PRIMARY KEY ====================

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // ==================== FOREIGN KEYS ====================

        builder.Property(p => p.DepartmentId)
            .HasColumnName("DepartmentId")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        // ==================== VALUE OBJECTS ====================

        // PersonName (Complex ValueObject)
        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(50)
                .IsRequired();

            name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Address (Complex ValueObject)
        builder.OwnsOne(p => p.Address, address =>
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

        // ==================== SCALAR PROPERTIES ====================

        // Kimlik Numarası (Unique)
        builder.Property(p => p.NationalId)
            .HasColumnName("NationalId")
            .HasColumnType("nvarchar(11)")
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(p => p.NationalId)
            .IsUnique()
            .HasDatabaseName("IX_Persons_NationalId_Unique");

        // Ad Soyad (Indexed for search)
        builder.HasIndex(p => new { p.Name })
            .HasDatabaseName("IX_Persons_Name");

        // Email (Unique)
        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasDatabaseName("IX_Persons_Email_Unique");

        // Telefon Numarası
        builder.Property(p => p.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => p.PhoneNumber)
            .HasDatabaseName("IX_Persons_PhoneNumber");

        // Doğum Tarihi
        builder.Property(p => p.BirthDate)
            .HasColumnName("BirthDate")
            .HasColumnType("datetime2")
            .IsRequired();

        // Cinsiyet (Enum)
        builder.Property(p => p.Gender)
            .HasColumnName("Gender")
            .HasConversion(
                v => (int)v,
                v => (Gender)v
            )
            .HasDefaultValue(Gender.Other);

        // Profil Fotoğrafı URL
        builder.Property(p => p.ProfilePhotoUrl)
            .HasColumnName("ProfilePhotoUrl")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired(false);

        // Departman ID (Indexed)
        builder.HasIndex(p => p.DepartmentId)
            .HasDatabaseName("IX_Persons_DepartmentId");

        // ==================== CHILD ENTITIES ====================

        // One-to-One: Student
        builder.HasOne(p => p.Student)
            .WithOne()
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // One-to-One: Staff
        builder.HasOne(p => p.Staff)
            .WithOne()
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // One-to-One: HealthRecord
        builder.HasOne(p => p.HealthRecord)
            .WithOne()
            .HasForeignKey<HealthRecord>(h => h.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // One-to-Many: Restrictions
        builder.HasMany<PersonRestriction>()
            .WithOne()
            .HasForeignKey(r => r.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // ==================== AUDIT FIELDS ====================

        // Soft Delete
        builder.Property(p => p.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false);

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Persons_IsDeleted");

        // Oluşturulma Tarihi
        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        // Güncelleme Tarihi
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();

        // Silme Tarihi
        builder.Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // ==================== CONCURRENCY ====================

        builder.Property(p => p.Id)
            .IsConcurrencyToken(false);

        // ==================== CONSTRAINTS & INDEXES ====================

        // Composite Index: DepartmentId + IsDeleted (for department queries)
        builder.HasIndex(p => new { p.DepartmentId, p.IsDeleted })
            .HasDatabaseName("IX_Persons_DepartmentId_IsDeleted");

        // Composite Index: Email + IsDeleted (for email lookups)
        builder.HasIndex(p => new { p.Email, p.IsDeleted })
            .HasDatabaseName("IX_Persons_Email_IsDeleted");

        // Composite Index: CreatedAt + IsDeleted (for audit trail)
        builder.HasIndex(p => new { p.CreatedAt, p.IsDeleted })
            .HasDatabaseName("IX_Persons_CreatedAt_IsDeleted");
    }
}