using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Academic;
public class CourseRegistrationConfiguration : IEntityTypeConfiguration<CourseRegistration>
{
    public void Configure(EntityTypeBuilder<CourseRegistration> builder)
    {
        builder.ToTable("CourseRegistrations");
        builder.HasKey(cr => cr.Id);
        builder.Property(cr => cr.StudentId)
            .IsRequired();
        builder.Property(cr => cr.CourseId)
            .IsRequired();
        builder.Property(cr => cr.Semester)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(cr => cr.RegistrationDate)
            .IsRequired();
        builder.Property(cr => cr.Status)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(cr => cr.DropDate);
        builder.Property(cr => cr.DropReason)
            .HasMaxLength(500);
        builder.Property(cr => cr.IsRetake)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(cr => cr.PreviousGradeId);
        builder.Property(cr => cr.GradeId);
        builder.Property(cr => cr.CreatedAt)
            .IsRequired();
        builder.Property(cr => cr.CreatedBy);
        builder.Property(cr => cr.UpdatedAt);
        builder.Property(cr => cr.UpdatedBy);
        builder.Property(cr => cr.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(cr => cr.DeletedAt);
        builder.Property(cr => cr.DeletedBy);
        builder.Property(cr => cr.RowVersion)
            .IsRowVersion();
        builder.HasIndex(cr => new { cr.StudentId, cr.CourseId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [Status] = 1");
        builder.HasIndex(cr => cr.StudentId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(cr => cr.CourseId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(cr => !cr.IsDeleted);
        builder.HasOne(cr => cr.Course)
            .WithMany()
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}