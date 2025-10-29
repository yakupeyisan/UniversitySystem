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

        // ============ Primary Key ============
        builder.HasKey(u => u.Id);

        // ============ Identity Properties ============

        // Email (ValueObject)
        builder
            .Property(u => u.Email)
            .HasConversion(
                e => e.Value,
                v => new Email(v).Value)
            .HasColumnName("Email")
            .HasMaxLength(256)
            .IsRequired()
            .HasComment("Kullanıcı email adresi");
        // PasswordHash (ValueObject)
        builder.OwnsOne(u => u.PasswordHash, ph =>
        {
            ph.Property(p => p.HashedPassword)
                .HasColumnName("PasswordHash")
                .IsRequired()
                .HasComment("Hashlenmiş şifre");

            ph.Property(p => p.Salt)
                .HasColumnName("PasswordSalt")
                .IsRequired()
                .HasComment("Şifreleme salt");
        });

        // FirstName
        builder
            .Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("Ad");

        // LastName
        builder
            .Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("Soyad");

        // ============ Email Verification Properties ============

        builder
            .Property(u => u.IsEmailVerified)
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("Email doğrulanmış mı");

        builder
            .Property(u => u.EmailVerificationCode)
            .HasMaxLength(100)
            .IsRequired(false)
            .HasComment("Email doğrulama kodu - 24 saat geçerli");

        builder
            .Property(u => u.EmailVerificationCodeExpiry)
            .IsRequired(false)
            .HasComment("Email doğrulama kodunun süresi dolma zamanı");

        // ============ Password Reset Properties ============

        builder
            .Property(u => u.PasswordResetCode)
            .HasMaxLength(100)
            .IsRequired(false)
            .HasComment("Şifre sıfırlama kodu - 1 saat geçerli");

        builder
            .Property(u => u.PasswordResetCodeExpiry)
            .IsRequired(false)
            .HasComment("Şifre sıfırlama kodunun süresi dolma zamanı");

        builder
            .Property(u => u.LastPasswordChangeAt)
            .IsRequired(false)
            .HasComment("Son şifre değiştirilme zamanı");

        // ============ Login & Access Properties ============

        builder
            .Property(u => u.LastLoginAt)
            .IsRequired(false)
            .HasComment("Son login zamanı");

        builder
            .Property(u => u.FailedLoginAttempts)
            .HasDefaultValue(0)
            .IsRequired()
            .HasComment("Başarısız login deneme sayısı");

        builder
            .Property(u => u.LockedUntil)
            .IsRequired(false)
            .HasComment("Hesabın kilitlenme süresi");

        builder
            .Property(u => u.Status)
            .HasConversion<int>()
            .HasComment("Kullanıcı durumu (0=Active, 1=Inactive, 2=Suspended, 3=Locked)");

        // ============ Soft Delete Properties ============

        builder
            .Property(u => u.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("Soft delete - silinmiş mi");

        builder
            .Property(u => u.DeletedAt)
            .IsRequired(false)
            .HasComment("Silinme zamanı");

        builder
            .Property(u => u.DeletedBy)
            .IsRequired(false)
            .HasComment("Silen kullanıcı ID'si");

        // ============ Audit Properties (AuditableEntity'den) ============

        builder
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired()
            .HasComment("Oluşturulma zamanı");

        builder
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired()
            .HasComment("Son güncelleme zamanı");

        // ============ Relationships ============

        // User -> Roles (Many-to-Many)
        builder
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserRoles"));

        // User -> Permissions (Many-to-Many, direkt)
        builder
            .HasMany(u => u.Permissions)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserPermissions"));

        // User -> RefreshTokens (One-to-Many)
        builder
            .HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RefreshTokens_Users_UserId");

        // ============ Indexes ============

        // Email arama
        builder
            .HasIndex(u => u.Email)
            .HasDatabaseName("IX_Users_Email")
            .IsUnique();

        // Email verification code arama
        builder
            .HasIndex(u => u.EmailVerificationCode)
            .HasDatabaseName("IX_Users_EmailVerificationCode");

        // Password reset code arama
        builder
            .HasIndex(u => u.PasswordResetCode)
            .HasDatabaseName("IX_Users_PasswordResetCode");

        // Status ve IsDeleted combined index
        builder
            .HasIndex(u => new { u.Status, u.IsDeleted })
            .HasDatabaseName("IX_Users_Status_IsDeleted");

        // IsEmailVerified index
        builder
            .HasIndex(u => u.IsEmailVerified)
            .HasDatabaseName("IX_Users_IsEmailVerified");

        // ============ Query Filters (Global Query Filter) ============

        // Soft delete - IsDeleted = false olan kayıtlar otomatik filtrelenir
        builder
            .HasQueryFilter(u => !u.IsDeleted);

        // ============ Properties Mapping ============

        // Shadow property (database tarafında var ama sınıfta yok)
        // İhtiyaç durumunda eklenebilir:
        // builder.Property<DateTime>("CreatedAtUtc");
    }
}