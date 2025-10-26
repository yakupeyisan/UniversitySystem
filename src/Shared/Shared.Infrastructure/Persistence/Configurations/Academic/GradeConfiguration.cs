using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Academic;

/// <summary>
/// Entity Framework Core configuration for Grade aggregate
/// </summary>
public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("Grades", "academic");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.StudentId)
            .IsRequired();

        builder.Property(g => g.CourseId)
            .IsRequired();

        builder.Property(g => g.RegistrationId)
            .IsRequired();

        builder.Property(g => g.Semester)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(g => g.MidtermScore)
            .IsRequired();

        builder.Property(g => g.FinalScore)
            .IsRequired();

        builder.Property(g => g.NumericScore)
            .IsRequired();

        builder.Property(g => g.LetterGrade)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(g => g.GradePoint)
            .IsRequired();

        builder.Property(g => g.ECTS)
            .IsRequired();

        builder.Property(g => g.IsObjected)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(g => g.ObjectionDeadline)
            .IsRequired();

        builder.Property(g => g.RecordedDate)
            .IsRequired();

        // Auditable properties
        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.CreatedBy);

        builder.Property(g => g.UpdatedAt);

        builder.Property(g => g.UpdatedBy);

        // Soft delete
        builder.Property(g => g.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(g => g.DeletedAt);

        builder.Property(g => g.DeletedBy);

        // Row version for concurrency
        builder.Property(g => g.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(g => new { g.StudentId, g.CourseId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(g => new { g.StudentId, g.Semester })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(g => g.StudentId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(g => g.RegistrationId)
            .HasFilter("[IsDeleted] = 0");

        // Global soft delete filter
        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}