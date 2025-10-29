using Academic.Domain.Aggregates;
using Academic.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Academic;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses", "academic");
        builder.HasKey(c => c.Id);
        builder
            .Property(c => c.Code)
            .HasConversion(
                c => c.Value,
                v => CourseCode.Create(v))
            .HasColumnName("CourseCode")
            .IsRequired();
        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(c => c.Description)
            .HasMaxLength(1000);
        builder.Property(c => c.ECTS)
            .IsRequired();
        builder.Property(c => c.Credits)
            .IsRequired();
        builder.Property(c => c.Level)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.Type)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.Semester)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(c => c.Year)
            .IsRequired();
        builder.Property(c => c.DepartmentId)
            .IsRequired();
        builder.Property(c => c.Status)
            .HasConversion<int>()
            .IsRequired();
        builder.OwnsOne(c => c.Capacity, co =>
        {
            co.Property(ci => ci.MaxCapacity)
                .HasColumnName("MaxCapacity")
                .IsRequired();
            co.Property(ci => ci.CurrentEnrollment)
                .HasColumnName("CurrentEnrollment")
                .IsRequired();
        });
        builder.Property<List<Guid>>("_instructorIds")
            .HasColumnName("InstructorIds")
            .HasConversion(
                v => string.Join(",", v),
                v => string.IsNullOrEmpty(v)
                    ? new List<Guid>()
                    : v.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .IsRequired();
        builder.Property<List<Guid>>("_prerequisiteIds")
            .HasColumnName("PrerequisiteIds")
            .HasConversion(
                v => string.Join(",", v),
                v => string.IsNullOrEmpty(v)
                    ? new List<Guid>()
                    : v.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList())
            .IsRequired();
        builder.Property(c => c.CreatedAt)
            .IsRequired();
        builder.Property(c => c.CreatedBy);
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy);
        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(c => c.DeletedAt);
        builder.Property(c => c.DeletedBy);
        builder.Property(c => c.RowVersion)
            .IsRowVersion();
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}