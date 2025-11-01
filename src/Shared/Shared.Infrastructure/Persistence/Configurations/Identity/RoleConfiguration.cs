using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles"); // Primary key
        builder.HasKey(r => r.Id);

        // Configure properties with constraints
        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        // Role name - Unique, Required, Max length 128
        builder.Property(r => r.RoleName)
            .HasColumnName("Name")
            .HasColumnType("nvarchar(128)")
            .IsRequired()
            .HasMaxLength(128);

        // Create unique index on Name for fast lookup
        builder.HasIndex(r => r.RoleName)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name_Unique")
            .HasFilter(null);

        // Role description - Optional, Max length 512
        builder.Property(r => r.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(512)")
            .IsRequired(false)
            .HasMaxLength(512);

        // Is system role - Cannot be deleted or modified
        builder.Property(r => r.IsSystemRole)
            .HasColumnName("IsSystemRole")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(false);

        // CreatedAt - Required datetime
        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // UpdatedAt - Required datetime
        builder.Property(r => r.UpdatedAt)
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

        // Relationships
        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity(
                "RolePermissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId").IsRequired(),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId").IsRequired(),
                j =>
                {
                    j.ToTable("RolePermissions", "identity");
                    j.HasKey("RoleId", "PermissionId");
                }
            );

        // Seed system roles
        builder.HasData(
            Role.Create("Admin", RoleType.Admin, "System administrator with full access", true),
            Role.Create("Staff", RoleType.Staff, "Regular staff member with limited access", true),
            Role.Create("Student", RoleType.Student, "Student user with restricted permissions", true),
            Role.Create("Faculty", RoleType.Faculty, "Faculty member with course and grading access", true),
            Role.Create("Guest", RoleType.Guest, "Guest user with minimal access", true),
            Role.Create("Moderator", RoleType.Moderator, "Moderator with content management permissions", true),
            Role.Create("Viewer", RoleType.Viewer, "Viewer role with read-only access", true)
        );
    }
}