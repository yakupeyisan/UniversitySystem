using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;

namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;

/// <summary>
/// EmergencyContact Entity Configuration
/// </summary>
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
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ec => ec.Relationship)
            .HasColumnName("Relationship")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ec => ec.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(ec => ec.ValidFrom)
            .HasColumnName("ValidFrom")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(ec => ec.ValidTo)
            .HasColumnName("ValidTo")
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(ec => ec.IsCurrent)
            .HasColumnName("IsCurrent")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(ec => ec.Priority)
            .HasColumnName("Priority")
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(ec => ec.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(ec => ec.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(ec => ec.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()");

        // Foreign Key constraint
        builder.HasOne<Person>()
            .WithMany(p => p.EmergencyContacts)
            .HasForeignKey(ec => ec.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for better query performance
        builder.HasIndex(ec => ec.PersonId);

        builder.HasIndex(ec => new { ec.PersonId, ec.IsCurrent, ec.IsDeleted })
            .HasName("IX_EmergencyContacts_PersonId_IsCurrent_IsDeleted");

        builder.HasIndex(ec => new { ec.PersonId, ec.Priority })
            .HasName("IX_EmergencyContacts_PersonId_Priority");

        builder.HasIndex(ec => new { ec.PersonId, ec.ValidFrom })
            .HasName("IX_EmergencyContacts_PersonId_ValidFrom");
    }
}