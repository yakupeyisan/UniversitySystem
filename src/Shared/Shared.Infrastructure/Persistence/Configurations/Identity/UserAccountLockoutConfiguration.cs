using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Identity;
public class UserAccountLockoutConfiguration : IEntityTypeConfiguration<UserAccountLockout>
{
    public void Configure(EntityTypeBuilder<UserAccountLockout> builder)
    {
        builder.ToTable("UserAccountLockouts");
        builder.HasKey(ual => ual.Id);
        builder.Property(ual => ual.Id)
            .HasColumnName("Id")
            .IsRequired();
        builder.Property(ual => ual.UserId)
            .HasColumnName("UserId")
            .IsRequired()
            .HasComment("Kilitli hesap");
        builder.Property(ual => ual.Reason)
            .HasColumnName("Reason")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired()
            .HasComment("Kilitleme nedeni");
        builder.Property(ual => ual.LockedAt)
            .HasColumnName("LockedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasComment("Kilitleme başlangıç zamanı");
        builder.Property(ual => ual.LockedUntil)
            .HasColumnName("LockedUntil")
            .HasColumnType("datetime2")
            .HasComment("Otomatik açılış zamanı");
        builder.Property(ual => ual.DurationType)
            .HasColumnName("DurationType")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("Minutes, Hours, Days, Permanent");
        builder.Property(ual => ual.DurationValue)
            .HasColumnName("DurationValue")
            .IsRequired()
            .HasComment("Süre değeri");
        builder.Property(ual => ual.ReasonDetails)
            .HasColumnName("ReasonDetails")
            .HasColumnType("nvarchar(1000)")
            .HasMaxLength(1000)
            .HasComment("Kilitleme sebebinin detayları");
        builder.Property(ual => ual.FailedAttemptCount)
            .HasColumnName("FailedAttemptCount")
            .IsRequired()
            .HasComment("Başarısız deneme sayısı");
        builder.Property(ual => ual.IpAddresses)
            .HasColumnName("IpAddresses")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .HasComment("IP adresleri (virgülle ayrılmış)");
        builder.Property(ual => ual.IsUnlocked)
            .HasColumnName("IsUnlocked")
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("Açılmış mı");
        builder.Property(ual => ual.UnlockedAt)
            .HasColumnName("UnlockedAt")
            .HasColumnType("datetime2")
            .HasComment("Açılış zamanı");
        builder.Property(ual => ual.UnlockReason)
            .HasColumnName("UnlockReason")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .HasComment("Açılış nedeni");
        builder.Property(ual => ual.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(ual => ual.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");
        builder.Property(ual => ual.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");
        builder.Property(ual => ual.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ual => ual.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_UserAccountLockouts_Users_UserId");
        builder.HasIndex(ual => ual.UserId)
            .HasDatabaseName("IX_UserAccountLockouts_UserId");
        builder.HasIndex(ual => ual.LockedAt)
            .HasDatabaseName("IX_UserAccountLockouts_LockedAt")
            .IsDescending();
        builder.HasIndex(ual => ual.LockedUntil)
            .HasDatabaseName("IX_UserAccountLockouts_LockedUntil");
        builder.HasIndex(ual => ual.Reason)
            .HasDatabaseName("IX_UserAccountLockouts_Reason");
        builder.HasIndex(ual => ual.IsUnlocked)
            .HasDatabaseName("IX_UserAccountLockouts_IsUnlocked");
        builder.HasIndex(ual => new { ual.UserId, ual.IsUnlocked })
            .HasDatabaseName("IX_UserAccountLockouts_UserId_IsUnlocked");
        builder.HasIndex(ual => new { ual.UserId, ual.LockedAt })
            .HasDatabaseName("IX_UserAccountLockouts_UserId_LockedAt")
            .IsDescending(false, true);
        builder.HasIndex(ual => new { ual.IsUnlocked, ual.LockedUntil })
            .HasDatabaseName("IX_UserAccountLockouts_IsUnlocked_LockedUntil");
    }
}