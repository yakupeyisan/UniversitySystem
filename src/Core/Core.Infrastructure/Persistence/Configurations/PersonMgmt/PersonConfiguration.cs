using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// Person Entity Configuration
/// </summary>
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    /// <summary>
    /// Configure Person entity
    /// </summary>
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");
        builder.HasKey(p => p.Id);

        // ==================== VALUE OBJECTS ====================

        // PersonName Value Object
        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(50);

            name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(50);
        });

        // Address Value Object
        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(255);

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100);

            address.Property(a => a.State)
                .HasColumnName("State")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .HasMaxLength(100);
        });

        // ==================== PROPERTIES ====================

        builder.Property(p => p.DepartmentId)
            .IsRequired(false);

        builder.Property(p => p.NationalId)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(p => p.BirthDate)
            .IsRequired();

        builder.Property(p => p.Gender)
            .IsRequired();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.ProfilePhotoUrl)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.DeletedAt)
            .IsRequired(false);

        // ==================== INDEXES ====================

        builder.HasIndex(p => p.NationalId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Persons_NationalId_Unique");

        builder.HasIndex(p => p.Email)
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Persons_Email");

        builder.HasIndex(p => p.DepartmentId)
            .HasDatabaseName("IX_Persons_DepartmentId");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Persons_CreatedAt");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Persons_IsDeleted");

        // ==================== RELATIONSHIPS ====================

        // One-to-One: Person -> Student
        builder.HasOne(p => p.Student)
            .WithOne()
            .HasForeignKey<Student>(s => s.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-One: Person -> Staff
        builder.HasOne(p => p.Staff)
            .WithOne()
            .HasForeignKey<Staff>(s => s.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-One: Person -> HealthRecord
        builder.HasOne(p => p.HealthRecord)
            .WithOne()
            .HasForeignKey<HealthRecord>(h => h.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: Person -> PersonRestrictions
        builder.HasMany(p => p.Restrictions)
            .WithOne(r => r.Person)
            .HasForeignKey(r => r.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}