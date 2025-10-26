using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Academic;

/// <summary>
/// Entity Framework Core configuration for CourseWaitingListEntry aggregate
/// </summary>
public class CourseWaitingListEntryConfiguration : IEntityTypeConfiguration<CourseWaitingListEntry>
{
    public void Configure(EntityTypeBuilder<CourseWaitingListEntry> builder)
    {
        builder.ToTable("CourseWaitingListEntries", "academic");

        builder.HasKey(wl => wl.Id);

        builder.Property(wl => wl.StudentId)
            .IsRequired();

        builder.Property(wl => wl.CourseId)
            .IsRequired();

        builder.Property(wl => wl.QueuePosition)
            .IsRequired();

        builder.Property(wl => wl.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(wl => wl.RequestedDate)
            .IsRequired();

        builder.Property(wl => wl.AdmittedDate);

        builder.Property(wl => wl.CancelledDate);

        builder.Property(wl => wl.CancelReason)
            .HasMaxLength(500);

        // Auditable properties
        builder.Property(wl => wl.CreatedAt)
            .IsRequired();

        builder.Property(wl => wl.CreatedBy);

        builder.Property(wl => wl.UpdatedAt);

        builder.Property(wl => wl.UpdatedBy);

        // Soft delete
        builder.Property(wl => wl.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(wl => wl.DeletedAt);

        builder.Property(wl => wl.DeletedBy);

        // Row version for concurrency
        builder.Property(wl => wl.RowVersion)
            .IsRowVersion();

        // Indexes
        builder.HasIndex(wl => new { wl.StudentId, wl.CourseId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(wl => new { wl.CourseId, wl.Status })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(wl => wl.StudentId)
            .HasFilter("[IsDeleted] = 0");

        // Global soft delete filter
        builder.HasQueryFilter(wl => !wl.IsDeleted);
    }
}