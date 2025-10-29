using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "identity");

        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(p => p.PermissionName)
            .HasColumnName("PermissionName")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(p => p.PermissionName)
            .IsUnique()
            .HasDatabaseName("IX_Permissions_PermissionName_Unique");

        builder.Property(p => p.PermissionType)
            .HasColumnName("PermissionType")
            .HasColumnType("int")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(p => p.PermissionType)
            .HasDatabaseName("IX_Permissions_PermissionType");

        builder.Property(p => p.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500);

        builder.Property(p => p.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Permissions_IsActive");

        // Audit Properties
        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(p => p.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        // Seed default permissions
        builder.HasData(
            Permission.Create("Create User", PermissionType.CreateUser, "Create new users in the system"),
            Permission.Create("Read User", PermissionType.ReadUser, "View user information"),
            Permission.Create("Update User", PermissionType.UpdateUser, "Update user information"),
            Permission.Create("Delete User", PermissionType.DeleteUser, "Delete users from the system"),
            Permission.Create("Create Role", PermissionType.CreateRole, "Create new roles"),
            Permission.Create("Read Role", PermissionType.ReadRole, "View role information"),
            Permission.Create("Update Role", PermissionType.UpdateRole, "Update role information"),
            Permission.Create("Delete Role", PermissionType.DeleteRole, "Delete roles"),
            Permission.Create("Create Permission", PermissionType.CreatePermission, "Create new permissions"),
            Permission.Create("Read Permission", PermissionType.ReadPermission, "View permission information"),
            Permission.Create("Update Permission", PermissionType.UpdatePermission, "Update permission information"),
            Permission.Create("Delete Permission", PermissionType.DeletePermission, "Delete permissions"),
            Permission.Create("Manage Person", PermissionType.ManagePerson, "Manage person records"),
            Permission.Create("View Person", PermissionType.ViewPerson, "View person information"),
            Permission.Create("Manage Academic", PermissionType.ManageAcademic, "Manage academic data"),
            Permission.Create("View Academic", PermissionType.ViewAcademic, "View academic information"),
            Permission.Create("View Dashboard", PermissionType.ViewDashboard, "Access dashboard"),
            Permission.Create("Export Data", PermissionType.ExportData, "Export system data"),
            Permission.Create("View Reports", PermissionType.ViewReports, "View system reports"),
            Permission.Create("Manage Settings", PermissionType.ManageSettings, "Manage system settings")
        );
    }
}