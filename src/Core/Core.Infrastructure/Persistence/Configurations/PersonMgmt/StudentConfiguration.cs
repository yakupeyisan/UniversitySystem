using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// Student Entity Configuration
/// </summary>
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    /// <summary>
    /// Configure Student entity
    /// </summary>
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.ProgramId)
            .IsRequired();

        builder.Property(s => s.EnrollmentDate)
            .IsRequired();

        builder.Property(s => s.EducationLevel)
            .IsRequired();

        builder.Property(s => s.StudentStatus)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.GPA)
            .IsRequired(false)
            .HasColumnType("decimal(3,2)");

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        // ==================== INDEXES ====================

        builder.HasIndex(s => s.StudentNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Students_StudentNumber_Unique");

        builder.HasIndex(s => s.ProgramId)
            .HasDatabaseName("IX_Students_ProgramId");

        builder.HasIndex(s => s.IsDeleted)
            .HasDatabaseName("IX_Students_IsDeleted");
    }
}