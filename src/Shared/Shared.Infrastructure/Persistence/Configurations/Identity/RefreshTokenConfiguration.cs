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

        // Configure properties with constraints
        builder.Property(rt => rt.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // UserId - Foreign key to User, Required
        builder.Property(rt => rt.UserId)
            .HasColumnName("UserId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        // Refresh token string - Required, Index for fast lookup
        builder.Property(rt => rt.Token)
            .HasColumnName("Token")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        // Create index on Token for fast lookup during refresh operations
        builder.HasIndex(rt => rt.Token)
            .HasDatabaseName("IX_RefreshTokens_Token");

        // ExpiresAt - Required datetime, tracks token expiration
        builder.Property(rt => rt.ExpiryDate)
            .HasColumnName("ExpiryDate")
            .HasColumnType("datetime2")
            .IsRequired();

        // RevokedAt - Optional datetime, null if not revoked
        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("RevokedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // IsRevoked - Computed property based on RevokedAt
        // Alternative: Store as boolean with default false
        builder.Property<bool>("IsRevoked")
            .HasColumnName("IsRevoked")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);

        // ReplacedByTokenId - FK to another RefreshToken (for token rotation audit trail)
        builder.Property<Guid?>("ReplacedByTokenId")
            .HasColumnName("ReplacedByTokenId")
            .HasColumnType("uniqueidentifier")
            .IsRequired(false);

        // IpAddress - Store client IP for security audit
        builder.Property(rt => rt.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("nvarchar(45)")
            .IsRequired(false)
            .HasMaxLength(45); // IPv6 can be up to 45 characters

        // UserAgent - Store user agent for security audit
        builder.Property(rt => rt.UserAgent)
            .HasColumnName("UserAgent")
            .HasColumnType("nvarchar(512)")
            .IsRequired(false)
            .HasMaxLength(512);

        // CreatedAt - Required datetime
        builder.Property(rt => rt.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // UpdatedAt - Optional datetime for revocation timestamp
        builder.Property(rt => rt.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .IsRequired(false);


        // Relationships
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Self-referential relationship for token rotation tracking
        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey("ReplacedByTokenId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(rt => new { rt.UserId, rt.ExpiryDate })
            .HasDatabaseName("IX_RefreshTokens_UserId_ExpiryDate");

        // Index for finding active tokens for a user
        builder.HasIndex(rt => new { rt.UserId, rt.RevokedAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_RevokedAt")
            .HasFilter("[RevokedAt] IS NULL"); // Partial index for active tokens only

        // Index for cleanup: find expired tokens
        builder.HasIndex(rt => rt.ExpiryDate)
            .HasDatabaseName("IX_RefreshTokens_ExpiryDate");

        // Comment for documentation
        builder.HasComment(
            "RefreshTokens table for managing long-lived refresh tokens used in authentication flow. " +
            "Includes revocation and expiration tracking for security.");
    }
}