using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Identity;
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();
        builder.Property(rt => rt.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        builder.Property(rt => rt.Token)
            .HasColumnName("Token")
            .HasColumnType("nvarchar(max)")
            .IsRequired();
        builder.HasIndex(rt => rt.Token)
            .HasDatabaseName("IX_RefreshTokens_Token");
        builder.Property(rt => rt.ExpiryDate)
            .HasColumnName("ExpiryDate")
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("RevokedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.Property<bool>("IsRevoked")
            .HasColumnName("IsRevoked")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property<Guid?>("ReplacedByTokenId")
            .HasColumnName("ReplacedByTokenId")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);
        builder.Property(rt => rt.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("nvarchar(45)")
            .IsRequired(false)
            .HasMaxLength(45);
        builder.Property(rt => rt.UserAgent)
            .HasColumnName("UserAgent")
            .HasColumnType("nvarchar(512)")
            .IsRequired(false)
            .HasMaxLength(512);
        builder.Property(rt => rt.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(rt => rt.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey("ReplacedByTokenId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(rt => new { rt.UserId, rt.ExpiryDate })
            .HasDatabaseName("IX_RefreshTokens_UserId_ExpiryDate");
        builder.HasIndex(rt => new { rt.UserId, rt.RevokedAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_RevokedAt")
            .HasFilter("[RevokedAt] IS NULL");
        builder.HasIndex(rt => rt.ExpiryDate)
            .HasDatabaseName("IX_RefreshTokens_ExpiryDate");
        builder.HasComment(
            "RefreshTokens table for managing long-lived refresh tokens used in authentication flow. " +
            "Includes revocation and expiration tracking for security.");
    }
}