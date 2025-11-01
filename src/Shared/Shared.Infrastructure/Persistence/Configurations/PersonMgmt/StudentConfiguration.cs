using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.HasOne<Person>()
            .WithOne(p => p.Student)
            .HasForeignKey<Student>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Property(s => s.StudentNumber)
            .HasColumnName("StudentNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(s => s.StudentNumber)
            .IsUnique()
            .HasDatabaseName("IX_Students_StudentNumber_Unique");
        builder.Property(s => s.EducationLevel)
            .HasColumnName("EducationLevel")
            .HasColumnType("int")
            .HasConversion(
                v => (int)v,
                v => (EducationLevel)v
            )
            .HasDefaultValue(EducationLevel.Bachelor)
            .IsRequired();
        builder.HasIndex(s => s.EducationLevel)
            .HasDatabaseName("IX_Students_EducationLevel");
        builder.Property(s => s.CurrentSemester)
            .HasColumnName("CurrentSemester")
            .HasColumnType("int")
            .HasDefaultValue(1)
            .IsRequired();
        builder.HasIndex(s => s.CurrentSemester)
            .HasDatabaseName("IX_Students_CurrentSemester");
        builder.Property(s => s.Status)
            .HasColumnName("Status")
            .HasColumnType("int")
            .HasConversion(
                v => (int)v,
                v => (StudentStatus)v
            )
            .HasDefaultValue(StudentStatus.Active)
            .IsRequired();
        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_Students_Status");
        builder.Property(s => s.CGPA)
            .HasColumnName("CGPA")
            .HasColumnType("float")
            .HasPrecision(10, 2)
            .HasDefaultValue(0.0)
            .IsRequired();
        builder.HasIndex(s => s.CGPA)
            .HasDatabaseName("IX_Students_CGPA");
        builder.Property(s => s.SGPA)
            .HasColumnName("SGPA")
            .HasColumnType("float")
            .HasPrecision(10, 2)
            .HasDefaultValue(0.0)
            .IsRequired();
        builder.Property(s => s.TotalCredits)
            .HasColumnName("TotalCredits")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();
        builder.Property(s => s.CompletedCredits)
            .HasColumnName("CompletedCredits")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();
        builder.HasIndex(s => s.CompletedCredits)
            .HasDatabaseName("IX_Students_CompletedCredits");
        builder.Property(s => s.EnrollmentDate)
            .HasColumnName("EnrollmentDate")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.HasIndex(s => s.EnrollmentDate)
            .HasDatabaseName("IX_Students_EnrollmentDate");
        builder.Property(s => s.GraduationDate)
            .HasColumnName("GraduationDate")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(s => s.GraduationDate)
            .HasDatabaseName("IX_Students_GraduationDate");
        builder.Property(s => s.AdvisorId)
            .HasColumnName("AdvisorId")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(s => s.AdvisorId)
            .HasDatabaseName("IX_Students_AdvisorId");
        builder.Property(s => s.ProgramId)
            .HasColumnName("ProgramId")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(s => s.ProgramId)
            .HasDatabaseName("IX_Students_ProgramId");
        builder.Property(s => s.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(s => s.IsDeleted)
            .HasDatabaseName("IX_Students_IsDeleted");
        builder.Property(s => s.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(s => s.DeletedAt)
            .HasDatabaseName("IX_Students_DeletedAt");
        builder.Property(s => s.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(s => s.DeletedBy)
            .HasDatabaseName("IX_Students_DeletedBy");
        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(s => s.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(s => s.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(s => new { s.Status, s.IsDeleted })
            .HasDatabaseName("IX_Students_Status_IsDeleted");
        builder.HasIndex(s => new { s.CGPA, s.Status, s.IsDeleted })
            .HasDatabaseName("IX_Students_CGPA_Status_IsDeleted");
        builder.HasIndex(s => new { s.EnrollmentDate, s.IsDeleted })
            .HasDatabaseName("IX_Students_EnrollmentDate_IsDeleted");
        builder.HasIndex(s => new { s.CurrentSemester, s.Status })
            .HasDatabaseName("IX_Students_CurrentSemester_Status");
        builder.HasIndex(s => new { s.Id, s.IsDeleted })
            .HasDatabaseName("IX_Students_PersonId_IsDeleted");
        builder.HasIndex(s => new { s.CreatedAt, s.IsDeleted })
            .HasDatabaseName("IX_Students_CreatedAt_IsDeleted");
    }
}