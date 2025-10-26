using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;

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

        builder.Property(pr => pr.PersonId)
            .HasColumnName("PersonId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasIndex(pr => pr.PersonId)
            .HasDatabaseName("IX_PersonRestrictions_PersonId");

        builder.Property(pr => pr.RestrictionType)
            .HasColumnName("RestrictionType")
            .HasColumnType("int")
            .HasConversion(
                v => (int)v,
                v => (RestrictionType)v
            )
            .IsRequired();

        builder.HasIndex(pr => pr.RestrictionType)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionType");

        builder.Property(pr => pr.RestrictionLevel)
            .HasColumnName("RestrictionLevel")
            .HasColumnType("int")
            .HasConversion(
                v => (int)v,
                v => (RestrictionLevel)v
            )
            .IsRequired();

        builder.HasIndex(pr => pr.RestrictionLevel)
            .HasDatabaseName("IX_PersonRestrictions_RestrictionLevel");

        builder.Property(pr => pr.AppliedBy)
            .HasColumnName("AppliedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasIndex(pr => pr.AppliedBy)
            .HasDatabaseName("IX_PersonRestrictions_AppliedBy");

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

        // ISoftDelete Properties
        builder.Property(pr => pr.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(pr => pr.IsDeleted)
            .HasDatabaseName("IX_PersonRestrictions_IsDeleted");

        builder.Property(pr => pr.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.HasIndex(pr => pr.DeletedAt)
            .HasDatabaseName("IX_PersonRestrictions_DeletedAt");

        builder.Property(pr => pr.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.HasIndex(pr => pr.DeletedBy)
            .HasDatabaseName("IX_PersonRestrictions_DeletedBy");

        // AuditableEntity Properties
        builder.Property(pr => pr.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(pr => pr.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        builder.Property(pr => pr.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(pr => pr.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        // Relationships
        builder.HasOne<Person>()
            .WithMany(p => p.Restrictions)
            .HasForeignKey(pr => pr.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite Indices for Query Optimization
        builder.HasIndex(pr => new { pr.PersonId, pr.IsActive, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_PersonId_IsActive_IsDeleted");

        builder.HasIndex(pr => new { pr.PersonId, pr.RestrictionLevel, pr.IsActive })
            .HasDatabaseName("IX_PersonRestrictions_PersonId_RestrictionLevel_IsActive");

        builder.HasIndex(pr => new { pr.PersonId, pr.StartDate, pr.EndDate })
            .HasDatabaseName("IX_PersonRestrictions_PersonId_DateRange");

        builder.HasIndex(pr => new { pr.RestrictionType, pr.RestrictionLevel, pr.IsActive })
            .HasDatabaseName("IX_PersonRestrictions_Type_Level_IsActive");

        builder.HasIndex(pr => new { pr.PersonId, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_PersonId_IsDeleted");

        builder.HasIndex(pr => new { pr.CreatedAt, pr.IsDeleted })
            .HasDatabaseName("IX_PersonRestrictions_CreatedAt_IsDeleted");
    }
}