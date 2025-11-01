using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Academic;
public class CourseWaitingListEntryConfiguration : IEntityTypeConfiguration<CourseWaitingListEntry>
{
    public void Configure(EntityTypeBuilder<CourseWaitingListEntry> builder)
    {
        builder.ToTable("CourseWaitingListEntries");
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
        builder.Property(wl => wl.CreatedAt)
            .IsRequired();
        builder.Property(wl => wl.CreatedBy);
        builder.Property(wl => wl.UpdatedAt);
        builder.Property(wl => wl.UpdatedBy);
        builder.Property(wl => wl.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(wl => wl.DeletedAt);
        builder.Property(wl => wl.DeletedBy);
        builder.Property(wl => wl.RowVersion)
            .IsRowVersion();
        builder.HasIndex(wl => new { wl.StudentId, wl.CourseId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(wl => new { wl.CourseId, wl.Status })
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(wl => wl.StudentId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(wl => !wl.IsDeleted);
        builder.HasOne(wl => wl.Course)
            .WithMany()
            .HasForeignKey(wl => wl.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}