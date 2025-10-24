using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;
public class PersonRestrictionConfiguration : IEntityTypeConfiguration<PersonRestriction>
{
    public void Configure(EntityTypeBuilder<PersonRestriction> builder)
    {
        builder.ToTable("PersonRestrictions", "PersonMgmt");
        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.Property(pr => pr.AppliedBy)
            .HasColumnName("AppliedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        builder.HasIndex(pr => pr.AppliedBy)
            .HasDatabaseName("IX_PersonRestrictions_AppliedBy");
        builder.Property(pr => pr.RestrictionType)
            .HasColumnName("RestrictionType")
            .HasConversion(
                v => (int)v,
                v => (RestrictionType)v
            )
            .IsRequired();
        builder.HasIndex(pr => pr.RestrictionType)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionType");
        builder.Property(pr => pr.RestrictionLevel)
            .HasColumnName("RestrictionLevel")
            .HasConversion(
                v => (int)v,
                v => (RestrictionLevel)v
            )
            .IsRequired();
        builder.HasIndex(pr => pr.RestrictionLevel)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionLevel");
        builder.Property(pr => pr.StartDate)
            .HasColumnName("StartDate")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.HasIndex(pr => pr.StartDate)
            .HasDatabaseName("IX_PersonRestrictions_StartDate");
        builder.Property(pr => pr.EndDate)
            .HasColumnName("EndDate")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(pr => pr.EndDate)
            .HasDatabaseName("IX_PersonRestrictions_EndDate");
        builder.Property(pr => pr.Reason)
            .HasColumnName("Reason")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(pr => pr.Severity)
            .HasColumnName("Severity")
            .HasColumnType("int")
            .IsRequired();
        builder.HasIndex(pr => pr.Severity)
            .HasDatabaseName("IX_PersonRestrictions_Severity");
        builder.Property(pr => pr.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();
        builder.HasIndex(pr => pr.IsActive)
            .HasDatabaseName("IX_PersonRestrictions_IsActive");
        builder.Property(pr => pr.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(pr => pr.IsDeleted)
            .HasDatabaseName("IX_PersonRestrictions_IsDeleted");
        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();
        builder.Property(pr => pr.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(pr => new { pr.RestrictionType, pr.IsActive, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_Type_Active_Deleted");
        builder.HasIndex(pr => new { pr.StartDate, pr.EndDate, pr.IsActive })
            .HasDatabaseName("IX_PersonRestrictions_DateRange_Active");
        builder.HasIndex(pr => new { pr.AppliedBy, pr.CreatedAt })
            .HasDatabaseName("IX_PersonRestrictions_AppliedBy_CreatedAt");
        builder.HasIndex(pr => new { pr.IsActive, pr.EndDate })
            .HasDatabaseName("IX_PersonRestrictions_Active_EndDate");
        builder.HasIndex(pr => new { pr.Severity, pr.RestrictionType })
            .HasDatabaseName("IX_PersonRestrictions_Severity_Type");
        builder.HasIndex(pr => new { pr.CreatedAt, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_CreatedAt_IsDeleted");
    }
}