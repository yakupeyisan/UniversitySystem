using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class TwoFactorTokenConfiguration : IEntityTypeConfiguration<TwoFactorToken>
{
    public void Configure(EntityTypeBuilder<TwoFactorToken> builder)
    {
        builder.ToTable("TwoFactorTokens");
        builder.HasKey(tft => tft.Id);

        builder.Property(tft => tft.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(tft => tft.UserId)
            .HasColumnName("UserId")
            .IsRequired()
            .HasComment("İlgili kullanıcı");

        builder.Property(tft => tft.Method)
            .HasColumnName("Method")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("TOTP, SMS, Email, FIDO2, WebAuthn");

        builder.Property(tft => tft.SecretKey)
            .HasColumnName("SecretKey")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .IsRequired()
            .HasComment("Şifrelenmiş gizli anahtar");

        builder.Property(tft => tft.BackupCodes)
            .HasColumnName("BackupCodes")
            .HasColumnType("nvarchar(max)")
            .IsRequired()
            .HasComment("Backup kodları (pipe ile ayrılmış)");

        builder.Property(tft => tft.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("2FA aktif mi");

        builder.Property(tft => tft.IsVerified)
            .HasColumnName("IsVerified")
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("Kullanıcı tarafından doğrulandı mı");

        builder.Property(tft => tft.VerifiedAt)
            .HasColumnName("VerifiedAt")
            .HasColumnType("datetime2")
            .HasComment("Doğrulama zamanı");

        builder.Property(tft => tft.LastUsedAt)
            .HasColumnName("LastUsedAt")
            .HasColumnType("datetime2")
            .HasComment("Son başarılı kullanım zamanı");

        builder.Property(tft => tft.DisabledAt)
            .HasColumnName("DisabledAt")
            .HasColumnType("datetime2")
            .HasComment("Devre dışı bırakılma zamanı");

        // Audit properties
        builder.Property(tft => tft.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(tft => tft.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(tft => tft.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(tft => tft.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        // Foreign Key
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(tft => tft.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TwoFactorTokens_Users_UserId");

        // Indexes
        builder.HasIndex(tft => tft.UserId)
            .HasDatabaseName("IX_TwoFactorTokens_UserId");

        builder.HasIndex(tft => tft.IsActive)
            .HasDatabaseName("IX_TwoFactorTokens_IsActive");

        builder.HasIndex(tft => tft.IsVerified)
            .HasDatabaseName("IX_TwoFactorTokens_IsVerified");

        builder.HasIndex(tft => tft.Method)
            .HasDatabaseName("IX_TwoFactorTokens_Method");

        builder.HasIndex(tft => new { tft.UserId, tft.IsActive, tft.IsVerified })
            .HasDatabaseName("IX_TwoFactorTokens_UserId_IsActive_IsVerified");

        builder.HasIndex(tft => new { tft.UserId, tft.IsActive })
            .HasDatabaseName("IX_TwoFactorTokens_UserId_IsActive");
    }
}