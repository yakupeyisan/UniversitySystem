using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
namespace Shared.Infrastructure.Persistence.Configurations.PersonMgmt;
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.Property(a => a.PersonId)
            .HasColumnName("PersonId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        builder.Property(a => a.Street)
            .HasColumnName("Street")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(a => a.City)
            .HasColumnName("City")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(a => a.Country)
            .HasColumnName("Country")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(a => a.PostalCode)
            .HasColumnName("PostalCode")
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired(false);
        builder.Property(a => a.ValidFrom)
            .HasColumnName("ValidFrom")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.HasIndex(a => a.ValidFrom)
            .HasDatabaseName("IX_Addresses_ValidFrom");
        builder.Property(a => a.ValidTo)
            .HasColumnName("ValidTo")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(a => a.ValidTo)
            .HasDatabaseName("IX_Addresses_ValidTo");
        builder.Property(a => a.IsCurrent)
            .HasColumnName("IsCurrent")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();
        builder.HasIndex(a => a.IsCurrent)
            .HasDatabaseName("IX_Addresses_IsCurrent");
        builder.Property(a => a.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(a => a.IsDeleted)
            .HasDatabaseName("IX_Addresses_IsDeleted");
        builder.Property(a => a.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasIndex(a => a.DeletedAt)
            .HasDatabaseName("IX_Addresses_DeletedAt");
        builder.Property(a => a.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasIndex(a => a.DeletedBy)
            .HasDatabaseName("IX_Addresses_DeletedBy");
        builder.Property(a => a.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
        builder.Property(a => a.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(a => a.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();
        builder.Property(a => a.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.HasOne<Person>()
            .WithMany(p => p.Addresses)
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(a => new { a.PersonId, a.IsCurrent, a.IsDeleted })
            .HasDatabaseName("IX_Addresses_PersonId_IsCurrent_IsDeleted");
        builder.HasIndex(a => new { a.PersonId, a.ValidFrom })
            .HasDatabaseName("IX_Addresses_PersonId_ValidFrom");
        builder.HasIndex(a => new { a.PersonId, a.ValidTo })
            .HasDatabaseName("IX_Addresses_PersonId_ValidTo");
        builder.HasIndex(a => new { a.PersonId, a.IsDeleted })
            .HasDatabaseName("IX_Addresses_PersonId_IsDeleted");
        builder.HasIndex(a => new { a.CreatedAt, a.IsDeleted })
            .HasDatabaseName("IX_Addresses_CreatedAt_IsDeleted");
    }
}