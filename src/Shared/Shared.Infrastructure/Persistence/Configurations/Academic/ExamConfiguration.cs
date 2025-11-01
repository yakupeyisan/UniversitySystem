using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Academic;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exams");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CourseId)
            .IsRequired();
        builder.Property(e => e.ExamType)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(e => e.ExamDate)
            .IsRequired();
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
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.CreatedBy);
        builder.Property(e => e.UpdatedAt);
        builder.Property(e => e.UpdatedBy);
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(e => e.DeletedAt);
        builder.Property(e => e.DeletedBy);
        builder.Property(e => e.RowVersion)
            .IsRowVersion();
        builder.HasIndex(e => e.CourseId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(e => new { e.ExamDate, e.Status })
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.HasOne(e => e.Course)
            .WithMany()
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}