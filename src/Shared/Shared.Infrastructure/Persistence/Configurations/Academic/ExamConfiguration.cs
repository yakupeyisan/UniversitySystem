using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Academic;

/// <summary>
/// Entity Framework Core configuration for Exam aggregate
/// </summary>
public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exams", "academic");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CourseId)
            .IsRequired();

        builder.Property(e => e.ExamType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.ExamDate)
            .IsRequired();

        // TimeSlot Value Object
        builder.OwnsOne(e => e.TimeSlot, ts =>
        {
            ts.Property(t => t.StartTime)
                .HasColumnName("StartTime")
                .IsRequired();

            ts.Property(t => t.EndTime)
                .HasColumnName("EndTime")
                .IsRequired();

            ts.Property(t => t.DurationMinutes)
                .HasColumnName("DurationMinutes")
                .IsRequired();
        });

        builder.Property(e => e.ExamRoomId);

        builder.Property(e => e.MaxCapacity)
            .IsRequired();

        builder.Property(e => e.CurrentRegisteredCount)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.IsOnline)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.OnlineLink)
            .HasMaxLength(500);

        // Auditable properties
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy);

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.UpdatedBy);

        // Soft delete
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt);

        builder.Property(e => e.DeletedBy);

        // Row version for concurrency
        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(e => e.CourseId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => new { e.ExamDate, e.Status })
            .HasFilter("[IsDeleted] = 0");

        // Global soft delete filter
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.HasOne(e => e.Course)
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}