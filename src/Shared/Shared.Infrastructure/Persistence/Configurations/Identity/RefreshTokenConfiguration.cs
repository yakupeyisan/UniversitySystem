using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "identity");

        builder.HasKey(rt => rt.Id);

        // Properties
        builder.Property(rt => rt.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(rt => rt.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        builder.Property(rt => rt.Token)
            .HasColumnName("Token")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token_Unique");

        builder.Property(rt => rt.ExpiryDate)
            .HasColumnName("ExpiryDate")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(rt => rt.ExpiryDate)
            .HasDatabaseName("IX_RefreshTokens_ExpiryDate");

        builder.Property(rt => rt.IsRevoked)
            .HasColumnName("IsRevoked")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(rt => rt.IsRevoked)
            .HasDatabaseName("IX_RefreshTokens_IsRevoked");

        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("RevokedAt")
            .HasColumnType("datetime2");

        builder.Property(rt => rt.RevokeReason)
            .HasColumnName("RevokeReason")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(rt => rt.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("nvarchar(45)")
            .HasMaxLength(45)
            .IsRequired();

        builder.Property(rt => rt.UserAgent)
            .HasColumnName("UserAgent")
            .HasColumnType("nvarchar(max)");

        // Audit Properties
        builder.Property(rt => rt.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(rt => rt.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(rt => rt.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(rt => rt.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        // Relationships
        builder.HasOne<User>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}