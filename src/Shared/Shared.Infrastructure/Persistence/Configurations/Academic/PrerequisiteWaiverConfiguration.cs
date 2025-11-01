using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Academic;
public class PrerequisiteWaiverConfiguration : IEntityTypeConfiguration<PrerequisiteWaiver>
{
    public void Configure(EntityTypeBuilder<PrerequisiteWaiver> builder)
    {
        builder.ToTable("PrerequisiteWaivers");
        builder.HasKey(pw => pw.Id);
        builder.Property(pw => pw.StudentId)
            .IsRequired();
        builder.Property(pw => pw.PrerequisiteId)
            .IsRequired();
        builder.Property(pw => pw.CourseId)
            .IsRequired();
        builder.Property(pw => pw.RequestedDate)
            .IsRequired();
        builder.Property(pw => pw.Reason)
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(pw => pw.Status)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(pw => pw.ApprovedBy);
        builder.Property(pw => pw.ApprovedDate);
        builder.Property(pw => pw.ApprovalNotes)
            .HasMaxLength(1000);
        builder.Property(pw => pw.ExpiryDate);
        builder.Property(pw => pw.CreatedAt)
            .IsRequired();
        builder.Property(pw => pw.CreatedBy);
        builder.Property(pw => pw.UpdatedAt);
        builder.Property(pw => pw.UpdatedBy);
        builder.Property(pw => pw.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(pw => pw.DeletedAt);
        builder.Property(pw => pw.DeletedBy);
        builder.Property(pw => pw.RowVersion)
            .IsRowVersion();
        builder.HasIndex(pw => new { pw.StudentId, pw.PrerequisiteId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(pw => pw.StudentId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(pw => pw.Status)
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(pw => !pw.IsDeleted);
        builder.HasOne(pw => pw.Prerequisite)
            .WithMany()
            .HasForeignKey(pw => pw.PrerequisiteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}