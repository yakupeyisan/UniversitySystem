using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Academic;

public class GradeObjectionConfiguration : IEntityTypeConfiguration<GradeObjection>
{
    public void Configure(EntityTypeBuilder<GradeObjection> builder)
    {
        builder.ToTable("GradeObjections", "academic");
        builder.HasKey(go => go.Id);
        builder.Property(go => go.GradeId)
            .IsRequired();
        builder.Property(go => go.StudentId)
            .IsRequired();
        builder.Property(go => go.CourseId)
            .IsRequired();
        builder.Property(go => go.ObjectionDate)
            .IsRequired();
        builder.Property(go => go.ObjectionDeadline)
            .IsRequired();
        builder.Property(go => go.Reason)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(go => go.Status)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(go => go.AppealLevel)
            .IsRequired();
        builder.Property(go => go.ReviewedBy);
        builder.Property(go => go.ReviewedDate);
        builder.Property(go => go.ReviewNotes)
            .HasMaxLength(1000);
        builder.Property(go => go.NewScore);
        builder.Property(go => go.NewLetterGrade)
            .HasConversion<int?>();
        builder.Property(go => go.CreatedAt)
            .IsRequired();
        builder.Property(go => go.CreatedBy);
        builder.Property(go => go.UpdatedAt);
        builder.Property(go => go.UpdatedBy);
        builder.Property(go => go.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(go => go.DeletedAt);
        builder.Property(go => go.DeletedBy);
        builder.Property(go => go.RowVersion)
            .IsRowVersion();
        builder.HasIndex(go => new { go.StudentId, go.GradeId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(go => go.GradeId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(go => go.StudentId)
            .HasFilter("[IsDeleted] = 0");
        builder.HasIndex(go => go.Status)
            .HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(go => !go.IsDeleted);
        builder.HasOne(go => go.Grade)
            .WithMany()
            .HasForeignKey(go => go.GradeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}