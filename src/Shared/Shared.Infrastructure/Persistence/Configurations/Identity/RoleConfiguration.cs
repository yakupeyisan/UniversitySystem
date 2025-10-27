using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "identity");

        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(r => r.RoleName)
            .HasColumnName("RoleName")
            .HasColumnType("nvarchar(128)")
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(r => r.RoleName)
            .IsUnique()
            .HasDatabaseName("IX_Roles_RoleName_Unique");

        builder.Property(r => r.RoleType)
            .HasColumnName("RoleType")
            .HasColumnType("int")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(r => r.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("IX_Roles_IsActive");

        builder.Property(r => r.IsSystemRole)
            .HasColumnName("IsSystemRole")
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(r => r.IsSystemRole)
            .HasDatabaseName("IX_Roles_IsSystemRole");

        // Audit Properties
        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(r => r.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

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
            Role.Create("Admin", RoleType.Admin, "System administrator with full access", isSystemRole: true),
            Role.Create("Staff", RoleType.Staff, "Regular staff member with limited access", isSystemRole: true),
            Role.Create("Student", RoleType.Student, "Student user with restricted permissions", isSystemRole: true),
            Role.Create("Faculty", RoleType.Faculty, "Faculty member with course and grading access", isSystemRole: true),
            Role.Create("Guest", RoleType.Guest, "Guest user with minimal access", isSystemRole: true),
            Role.Create("Moderator", RoleType.Moderator, "Moderator with content management permissions", isSystemRole: true),
            Role.Create("Viewer", RoleType.Viewer, "Viewer role with read-only access", isSystemRole: true)
        );
    }
}