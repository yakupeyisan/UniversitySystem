using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonMgmt.Domain.Aggregates;
namespace Core.Infrastructure.Persistence.Configurations.PersonMgmt;
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses", "PersonMgmt");
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
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(a => a.City)
            .HasColumnName("City")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(a => a.Country)
            .HasColumnName("Country")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(a => a.PostalCode)
            .HasColumnName("PostalCode")
            .HasMaxLength(20)
            .IsRequired(false);
        builder.Property(a => a.ValidFrom)
            .HasColumnName("ValidFrom")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(a => a.ValidTo)
            .HasColumnName("ValidTo")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.Property(a => a.IsCurrent)
            .HasColumnName("IsCurrent")
            .HasDefaultValue(true)
            .IsRequired();
        builder.Property(a => a.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(a => a.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(a => a.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()");
        builder.HasOne<Person>()
            .WithMany(p => p.Addresses)
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(a => a.PersonId);
        builder.HasIndex(a => new { a.PersonId, a.IsCurrent, a.IsDeleted })
            .HasName("IX_Addresses_PersonId_IsCurrent_IsDeleted");
        builder.HasIndex(a => new { a.PersonId, a.ValidFrom })
            .HasName("IX_Addresses_PersonId_ValidFrom");
    }
}