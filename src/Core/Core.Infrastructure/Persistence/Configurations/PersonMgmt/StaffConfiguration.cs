using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// Staff Entity Configuration
/// </summary>
public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    /// <summary>
    /// Configure Staff entity
    /// </summary>
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Position)
            .IsRequired();

        builder.Property(s => s.HireDate)
            .IsRequired();

        builder.Property(s => s.DepartmentId)
            .IsRequired(false);

        builder.Property(s => s.Salary)
            .IsRequired(false)
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.EmploymentStatus)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        // ==================== INDEXES ====================

        builder.HasIndex(s => s.EmployeeNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Staff_EmployeeNumber_Unique");

        builder.HasIndex(s => s.DepartmentId)
            .HasDatabaseName("IX_Staff_DepartmentId");

        builder.HasIndex(s => s.IsDeleted)
            .HasDatabaseName("IX_Staff_IsDeleted");
    }
}