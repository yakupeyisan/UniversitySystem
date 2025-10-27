using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;
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
        builder.HasIndex(p => p.DepartmentId)
            .HasDatabaseName("IX_Persons_DepartmentId");
        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasColumnType("nvarchar(50)")
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
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Persons_IdentificationNumber_Unique");
        builder.Property(p => p.BirthDate)
            .HasColumnName("BirthDate")
            .HasColumnType("date")
            .IsRequired();
        builder.HasIndex(p => p.BirthDate)
            .HasDatabaseName("IX_Persons_BirthDate");
        builder.Property(p => p.Gender)
            .HasColumnName("Gender")
            .HasColumnType("nvarchar(50)")
            .IsRequired();
        builder.HasIndex(p => p.Gender)
            .HasDatabaseName("IX_Persons_Gender");
        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Persons_Email_Unique");
        builder.Property(p => p.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(p => p.PhoneNumber)
            .HasDatabaseName("IX_Persons_PhoneNumber");
        builder.Property(p => p.ProfilePhotoUrl)
            .HasColumnName("ProfilePhotoUrl")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired(false);
        builder.Property(p => p.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Persons_IsDeleted");
        builder.Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(p => p.DeletedAt)
            .HasDatabaseName("IX_Persons_DeletedAt");
        builder.Property(p => p.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(p => p.DeletedBy)
            .HasDatabaseName("IX_Persons_DeletedBy");
        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(p => p.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(p => p.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasOne(p => p.Student)
            .WithOne()
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.Staff)
            .WithOne()
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.HealthRecord)
            .WithOne()
            .HasForeignKey<HealthRecord>(h => h.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Addresses)
            .WithOne()
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.EmergencyContacts)
            .WithOne()
            .HasForeignKey(ec => ec.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Restrictions)
            .WithOne()
            .HasForeignKey(pr => pr.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(p => new { p.IsDeleted, p.CreatedAt })
            .HasDatabaseName("IX_Persons_IsDeleted_CreatedAt");
        builder.HasIndex(p => new { p.Email, p.IsDeleted })
            .HasDatabaseName("IX_Persons_Email_IsDeleted");
        builder.HasIndex(p => new { p.DepartmentId, p.IsDeleted })
            .HasDatabaseName("IX_Persons_DepartmentId_IsDeleted");
        builder.HasIndex(p => new { p.CreatedAt, p.IsDeleted })
            .HasDatabaseName("IX_Persons_CreatedAt_IsDeleted");
    }
}