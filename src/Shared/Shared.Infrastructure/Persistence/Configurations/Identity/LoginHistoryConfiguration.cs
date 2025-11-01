using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

public class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("LoginHistories");
        builder.HasKey(lh => lh.Id);

        builder.Property(lh => lh.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(lh => lh.UserId)
            .HasColumnName("UserId")
            .IsRequired()
            .HasComment("Giriş yapan kullanıcı");

        builder.Property(lh => lh.LoginAt)
            .HasColumnName("LoginAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasComment("Giriş zamanı");

        builder.Property(lh => lh.LogoutAt)
            .HasColumnName("LogoutAt")
            .HasColumnType("datetime2")
            .HasComment("Çıkış zamanı");

        builder.Property(lh => lh.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("nvarchar(45)")
            .HasMaxLength(45)
            .IsRequired()
            .HasComment("Giriş yapılan IP adresi");

        builder.Property(lh => lh.UserAgent)
            .HasColumnName("UserAgent")
            .HasColumnType("nvarchar(max)")
            .IsRequired()
            .HasComment("Tarayıcı/Cihaz bilgisi");

        builder.Property(lh => lh.OperatingSystem)
            .HasColumnName("OperatingSystem")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .HasComment("İşletim sistemi");

        builder.Property(lh => lh.BrowserName)
            .HasColumnName("BrowserName")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .HasComment("Tarayıcı adı");

        builder.Property(lh => lh.DeviceType)
            .HasColumnName("DeviceType")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .HasComment("Cihaz tipi (Desktop/Mobile/Tablet)");

        builder.Property(lh => lh.Location)
            .HasColumnName("Location")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .HasComment("Coğrafi konum");

        builder.Property(lh => lh.Result)
            .HasColumnName("Result")
            .HasColumnType("nvarchar(50)")
            .HasMaxLength(50)
            .IsRequired()
            .HasComment("Success/Failed/Locked");

        builder.Property(lh => lh.ErrorMessage)
            .HasColumnName("ErrorMessage")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .HasComment("Hata mesajı (varsa)");

        builder.Property(lh => lh.SessionDurationMinutes)
            .HasColumnName("SessionDurationMinutes")
            .HasComment("Oturum süresi (dakika)");

        builder.Property(lh => lh.IsTwoFactorUsed)
            .HasColumnName("IsTwoFactorUsed")
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("2FA kullanıldı mı");

        builder.Property(lh => lh.AccessTokenCreated)
            .HasColumnName("AccessTokenCreated")
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("Access token oluşturuldu mu");

        builder.Property(lh => lh.RefreshTokenCreated)
            .HasColumnName("RefreshTokenCreated")
            .HasDefaultValue(false)
            .IsRequired()
            .HasComment("Refresh token oluşturuldu mu");

        // Audit properties
        builder.Property(lh => lh.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(lh => lh.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(lh => lh.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(lh => lh.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        // Foreign Key
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(lh => lh.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_LoginHistories_Users_UserId");

        // Indexes
        builder.HasIndex(lh => lh.UserId)
            .HasDatabaseName("IX_LoginHistories_UserId");

        builder.HasIndex(lh => lh.LoginAt)
            .HasDatabaseName("IX_LoginHistories_LoginAt")
            .IsDescending();

        builder.HasIndex(lh => lh.IpAddress)
            .HasDatabaseName("IX_LoginHistories_IpAddress");

        builder.HasIndex(lh => lh.Result)
            .HasDatabaseName("IX_LoginHistories_Result");

        builder.HasIndex(lh => new { lh.UserId, lh.LoginAt })
            .HasDatabaseName("IX_LoginHistories_UserId_LoginAt")
            .IsDescending(false, true);

        builder.HasIndex(lh => new { lh.IpAddress, lh.LoginAt })
            .HasDatabaseName("IX_LoginHistories_IpAddress_LoginAt")
            .IsDescending(false, true);
    }
}