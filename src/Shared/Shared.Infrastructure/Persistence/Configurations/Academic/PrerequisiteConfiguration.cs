using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Academic;
public class PrerequisiteConfiguration : IEntityTypeConfiguration<Prerequisite>
{
    public void Configure(EntityTypeBuilder<Prerequisite> builder)
    {
        builder.ToTable("Prerequisites", "academic");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.CourseId)
            .IsRequired();
        builder.Property(p => p.PrerequisiteCourseId)
            .IsRequired();
        builder.Property(p => p.MinimumGrade)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(p => p.IsRequired)
            .IsRequired();
        builder.Property(p => p.WaiverAllowed)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.CreatedBy);
        builder.Property(p => p.UpdatedAt);
        builder.Property(p => p.UpdatedBy);
        builder.HasIndex(p => new { p.CourseId, p.PrerequisiteCourseId })
            .IsUnique();
        builder.HasIndex(p => p.PrerequisiteCourseId);
    }
}