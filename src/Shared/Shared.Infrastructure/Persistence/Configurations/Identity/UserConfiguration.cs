using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "identity");

        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // Email ValueObject
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasColumnType("nvarchar(256)")
                .HasMaxLength(256)
                .IsRequired();
        });

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique")
            .HasFilter("[IsDeleted] = 0");

        // PasswordHash ValueObject
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

        // Basic Properties
        builder.Property(u => u.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("LastName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasColumnName("Status")
            .HasColumnType("int")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Users_Status");

        builder.Property(u => u.IsEmailVerified)
            .HasColumnName("IsEmailVerified")
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("LastLoginAt")
            .HasColumnType("datetime2");

        builder.Property(u => u.FailedLoginAttempts)
            .HasColumnName("FailedLoginAttempts")
            .HasColumnType("int")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(u => u.LockedUntil)
            .HasColumnName("LockedUntil")
            .HasColumnType("datetime2");

        // Soft Delete
        builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(u => u.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("datetime2");

        builder.Property(u => u.DeletedBy)
            .HasColumnName("DeletedBy")
            .HasColumnType("uniqueidentifier");

        builder.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        // Audit Properties
        builder.Property(u => u.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(u => u.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        // Relationships (Many-to-Many with Join Tables)
        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity(
                "UserRoles",
                l => l.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId").IsRequired(),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").IsRequired(),
                j => j.ToTable("UserRoles", "identity")
            );

        builder.HasMany(u => u.Permissions)
            .WithMany()
            .UsingEntity(
                "UserPermissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId").IsRequired(),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").IsRequired(),
                j => j.ToTable("UserPermissions", "identity")
            );

        // Refresh Token Relationship
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}