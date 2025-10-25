using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons", "PersonMgmt");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(p => p.DepartmentId)
            .HasColumnName("DepartmentId")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        // Owned type: PersonName
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

        // Owned type: Address
        builder.OwnsOne(p => p.Address, b =>
        {
            b.Property(a => a.Street).HasColumnName("Street");
            b.Property(a => a.City).HasColumnName("City");
            b.Property(a => a.PostalCode).HasColumnName("PostalCode");
            b.Property(a => a.Country).HasColumnName("Country");
            b.Property(a => a.FullAddress).HasColumnName("FullAddress");
        });

        builder.Property(p => p.IdentificationNumber)
            .HasColumnName("IdentificationNumber")
            .HasColumnType("nvarchar(11)")
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(p => p.IdentificationNumber)
            .IsUnique()
            .HasDatabaseName("IX_Persons_IdentificationNumber_Unique");

        builder.Property(p => p.BirthDate)
            .HasColumnName("BirthDate")
            .HasColumnType("date")
            .IsRequired();

        builder.HasIndex(p => p.BirthDate)
            .HasDatabaseName("IX_Persons_BirthDate");

        builder.Property(p => p.Gender)
            .HasColumnName("Gender")
            .HasConversion(
                v => (int)v,
                v => (Gender)v
            )
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasDatabaseName("IX_Persons_Email_Unique");

        builder.Property(p => p.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.ProfilePhotoUrl)
            .HasColumnName("ProfilePhotoUrl")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired(false);

        // Relationships: One-to-One
        builder.HasOne(p => p.Student)
            .WithOne()
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(p => p.Staff)
            .WithOne()
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(p => p.HealthRecord)
            .WithOne()
            .HasForeignKey<HealthRecord>(h => h.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Relationship: One-to-Many with PersonRestriction
        // FIXED: Changed from HasForeignKey(r => r.Id) to HasForeignKey(r => r.PersonId)
        builder.HasMany<PersonRestriction>()
            .WithOne()
            .HasForeignKey(r => r.PersonId)  // ← DÜZELTME: PersonId kullanılıyor
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.Property(p => p.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false);

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Persons_IsDeleted");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(p => p.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        // Composite indexes for better performance
        builder.HasIndex(p => new { p.DepartmentId, p.IsDeleted })
            .HasDatabaseName("IX_Persons_DepartmentId_IsDeleted");

        builder.HasIndex(p => new { p.Email, p.IsDeleted })
            .HasDatabaseName("IX_Persons_Email_IsDeleted");

        builder.HasIndex(p => new { p.CreatedAt, p.IsDeleted })
            .HasDatabaseName("IX_Persons_CreatedAt_IsDeleted");
    }
}


