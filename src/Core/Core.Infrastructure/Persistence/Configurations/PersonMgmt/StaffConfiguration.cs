using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;
public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff", "PersonMgmt");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.HasOne<Person>()
            .WithOne(p => p.Staff)
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Property(s => s.EmployeeNumber)
            .HasColumnName("EmployeeNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(s => s.EmployeeNumber)
            .IsUnique()
            .HasDatabaseName("IX_Staff_EmployeeNumber_Unique");
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
        builder.Property(s => s.HireDate)
            .HasColumnName("HireDate")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.HasIndex(s => s.HireDate)
            .HasDatabaseName("IX_Staff_HireDate");
        builder.Property(s => s.TerminationDate)
            .HasColumnName("TerminationDate")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(s => s.TerminationDate)
            .HasDatabaseName("IX_Staff_TerminationDate");
        builder.Property(s => s.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();
        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IX_Staff_IsActive");
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
        builder.Property(s => s.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(s => s.IsDeleted)
            .HasDatabaseName("IX_Staff_IsDeleted");
        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(s => new { s.IsActive, s.IsDeleted })
            .HasDatabaseName("IX_Staff_IsActive_IsDeleted");
        builder.HasIndex(s => new { s.AcademicTitle, s.IsActive })
            .HasDatabaseName("IX_Staff_AcademicTitle_IsActive");
        builder.HasIndex(s => new { s.HireDate, s.IsActive })
            .HasDatabaseName("IX_Staff_HireDate_IsActive");
        builder.HasIndex(s => new { s.TerminationDate, s.IsDeleted })
            .HasDatabaseName("IX_Staff_TerminationDate_IsDeleted");
    }
}