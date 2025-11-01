using Identity.Domain.Aggregates;
using Identity.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // ============ Table Mapping ============
        builder.ToTable("Users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Configure properties with constraints
        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // Email - Unique, Required, Max length 256
        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(256)")
            .IsRequired()
            .HasMaxLength(256);

        // Create unique index on Email for fast lookup and constraint
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique")
            .HasFilter(null);

        // FullName - Required, Max length 256
        builder.Property(u => u.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("nvarchar(256)")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.LastName)
            .HasColumnName("LastName")
            .HasColumnType("nvarchar(256)")
            .IsRequired()
            .HasMaxLength(256);

        // IsActive - Required
        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(true);

        // IsDeleted - Required (soft delete pattern)
        builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);

        // IsLocked - Required
        builder.Property(u => u.IsLocked)
            .HasColumnName("IsLocked")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);

        // LastLoginAt - Nullable datetime
        builder.Property(u => u.LastLoginAt)
            .HasColumnName("LastLoginAt")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // CreatedAt - Required datetime
        builder.Property(u => u.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // UpdatedAt - Required datetime
        builder.Property(u => u.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Configure shadow properties for audit tracking
        builder.Property<string>("CreatedBy")
            .HasColumnName("CreatedBy")
            .HasColumnType("nvarchar(256)")
            .IsRequired(false);

        builder.Property<string>("UpdatedBy")
            .HasColumnName("UpdatedBy")
            .HasColumnType("nvarchar(256)")
            .IsRequired(false);

        // Owned entity: PasswordHash (Value Object)
        // Stored as shadow properties (not separate table)
        builder.OwnsOne(u => u.PasswordHash, ph =>
        {
            ph.Property(p => p.HashedPassword)
                .HasColumnName("PasswordHash")
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            ph.Property(p => p.Salt)
                .HasColumnName("PasswordSalt")
                .HasColumnType("nvarchar(max)")
                .IsRequired();
        });

        // Failed login attempts counter - stored as simple property
        builder.Property<int>("FailedLoginAttempts")
            .HasColumnName("FailedLoginAttempts")
            .HasColumnType("int")
            .IsRequired()
            .HasDefaultValue(0);

        // Account lock expiration time
        builder.Property<DateTime?>("LockoutUntil")
            .HasColumnName("LockoutUntil")
            .HasColumnType("datetime2")
            .IsRequired(false);

        // Relationships
        // One User to Many RefreshTokens (cascade delete)
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Roles)
            .WithMany()
            .UsingEntity(
                "UserRoles",
                l => l.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").IsRequired(),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId").IsRequired(),
                j =>
                {
                    j.ToTable("UserRoles");
                    j.HasKey("UserId", "RoleId");
                }
            );
        builder.HasMany(u => u.Permissions)
            .WithMany()
            .UsingEntity(
                "UserPermissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId").IsRequired(),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").IsRequired(),
                j =>
                {
                    j.ToTable("UserPermissions");
                    j.HasKey("RoleId", "UserId");
                }
            );
        // Global query filter for soft delete pattern
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Configure concurrency token for optimistic concurrency control
        builder.Property(u => u.UpdatedAt)
            .IsConcurrencyToken();

        // Indexes for common queries
        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        builder.HasIndex(u => new { u.IsActive, u.IsDeleted })
            .HasDatabaseName("IX_Users_ActiveDeleted");

        builder.HasIndex(u => u.LastLoginAt)
            .HasDatabaseName("IX_Users_LastLoginAt");

        // Comment for documentation
        builder.HasComment(
            "Users table for authentication and authorization. Contains user credentials and account status.");
    }
}