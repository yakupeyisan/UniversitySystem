using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");
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
            .HasColumnType("int")
            .HasConversion(
                v => (int)v,
                v => (AcademicTitle)v
            )
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
        builder.Property(s => s.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(s => s.IsDeleted)
            .HasDatabaseName("IX_Staff_IsDeleted");
        builder.Property(s => s.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(s => s.DeletedAt)
            .HasDatabaseName("IX_Staff_DeletedAt");
        builder.Property(s => s.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(s => s.DeletedBy)
            .HasDatabaseName("IX_Staff_DeletedBy");
        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(s => s.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(s => s.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(s => new { s.IsActive, s.IsDeleted })
            .HasDatabaseName("IX_Staff_IsActive_IsDeleted");
        builder.HasIndex(s => new { s.AcademicTitle, s.IsActive })
            .HasDatabaseName("IX_Staff_AcademicTitle_IsActive");
        builder.HasIndex(s => new { s.HireDate, s.IsActive })
            .HasDatabaseName("IX_Staff_HireDate_IsActive");
        builder.HasIndex(s => new { s.TerminationDate, s.IsDeleted })
            .HasDatabaseName("IX_Staff_TerminationDate_IsDeleted");
        builder.HasIndex(s => new { s.AcademicTitle, s.IsActive, s.IsDeleted })
            .HasDatabaseName("IX_Staff_AcademicTitle_IsActive_IsDeleted");
        builder.HasIndex(s => new { s.HireDate, s.IsDeleted })
            .HasDatabaseName("IX_Staff_HireDate_IsDeleted");
        builder.HasIndex(s => new { s.CreatedAt, s.IsDeleted })
            .HasDatabaseName("IX_Staff_CreatedAt_IsDeleted");
    }
}