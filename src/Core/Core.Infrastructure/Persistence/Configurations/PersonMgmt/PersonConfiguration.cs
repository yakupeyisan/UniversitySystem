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

        builder.Property(p => p.IdentificationNumber)
            .HasColumnName("IdentificationNumber")
            .HasColumnType("nvarchar(11)")
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(p => p.IdentificationNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(p => p.BirthDate)
            .HasColumnName("BirthDate")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasColumnName("Gender")
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(p => p.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.ProfilePhotoUrl)
            .HasColumnName("ProfilePhotoUrl")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(p => p.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt")
            .IsRequired(false);

        builder.Property(p => p.DeletedBy)
            .HasColumnName("DeletedBy")
            .IsRequired(false);

        // ✅ ONE-TO-MANY: Person -> Addresses
        builder.HasMany(p => p.Addresses)
            .WithOne()
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ ONE-TO-MANY: Person -> PersonRestrictions
        builder.HasMany(p => p.Restrictions)
            .WithOne()
            .HasForeignKey(pr => pr.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ONE-TO-ONE: Person -> Student
        builder.HasOne(p => p.Student)
            .WithOne()
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // ONE-TO-ONE: Person -> Staff
        builder.HasOne(p => p.Staff)
            .WithOne()
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasMany(p => p.EmergencyContacts)
            .WithOne()
            .HasForeignKey(ec => ec.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // ONE-TO-ONE: Person -> HealthRecord
        builder.HasOne(p => p.HealthRecord)
            .WithOne()
            .HasForeignKey<HealthRecord>(h => h.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}