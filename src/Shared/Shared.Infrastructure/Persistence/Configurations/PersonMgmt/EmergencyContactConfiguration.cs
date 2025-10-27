using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;
public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("EmergencyContacts", "PersonMgmt");
        builder.HasKey(ec => ec.Id);
        builder.Property(ec => ec.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.Property(ec => ec.PersonId)
            .HasColumnName("PersonId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        builder.Property(ec => ec.FullName)
            .HasColumnName("FullName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(ec => ec.FullName)
            .HasDatabaseName("IX_EmergencyContacts_FullName");
        builder.Property(ec => ec.Relationship)
            .HasColumnName("Relationship")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(ec => ec.Relationship)
            .HasDatabaseName("IX_EmergencyContacts_Relationship");
        builder.Property(ec => ec.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(ec => ec.PhoneNumber)
            .HasDatabaseName("IX_EmergencyContacts_PhoneNumber");
        builder.Property(ec => ec.ValidFrom)
            .HasColumnName("ValidFrom")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.HasIndex(ec => ec.ValidFrom)
            .HasDatabaseName("IX_EmergencyContacts_ValidFrom");
        builder.Property(ec => ec.ValidTo)
            .HasColumnName("ValidTo")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(ec => ec.ValidTo)
            .HasDatabaseName("IX_EmergencyContacts_ValidTo");
        builder.Property(ec => ec.IsCurrent)
            .HasColumnName("IsCurrent")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();
        builder.HasIndex(ec => ec.IsCurrent)
            .HasDatabaseName("IX_EmergencyContacts_IsCurrent");
        builder.Property(ec => ec.Priority)
            .HasColumnName("Priority")
            .HasColumnType("int")
            .HasDefaultValue(1)
            .IsRequired();
        builder.HasIndex(ec => ec.Priority)
            .HasDatabaseName("IX_EmergencyContacts_Priority");
        builder.Property(ec => ec.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(ec => ec.IsDeleted)
            .HasDatabaseName("IX_EmergencyContacts_IsDeleted");
        builder.Property(ec => ec.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(ec => ec.DeletedAt)
            .HasDatabaseName("IX_EmergencyContacts_DeletedAt");
        builder.Property(ec => ec.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(ec => ec.DeletedBy)
            .HasDatabaseName("IX_EmergencyContacts_DeletedBy");
        builder.Property(ec => ec.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(ec => ec.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(ec => ec.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(ec => ec.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasOne<Person>()
            .WithMany(p => p.EmergencyContacts)
            .HasForeignKey(ec => ec.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(ec => new { ec.PersonId, ec.IsCurrent, ec.IsDeleted })
            .HasDatabaseName("IX_EmergencyContacts_PersonId_IsCurrent_IsDeleted");
        builder.HasIndex(ec => new { ec.PersonId, ec.Priority })
            .HasDatabaseName("IX_EmergencyContacts_PersonId_Priority");
        builder.HasIndex(ec => new { ec.PersonId, ec.ValidFrom })
            .HasDatabaseName("IX_EmergencyContacts_PersonId_ValidFrom");
        builder.HasIndex(ec => new { ec.PersonId, ec.IsDeleted })
            .HasDatabaseName("IX_EmergencyContacts_PersonId_IsDeleted");
        builder.HasIndex(ec => new { ec.CreatedAt, ec.IsDeleted })
            .HasDatabaseName("IX_EmergencyContacts_CreatedAt_IsDeleted");
    }
}