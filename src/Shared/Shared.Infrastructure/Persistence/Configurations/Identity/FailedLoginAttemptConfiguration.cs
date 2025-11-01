using Identity.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Persistence.Configurations.Identity;

/// <summary>
/// FailedLoginAttempt Entity Configuration
/// Başarısız giriş denemelerini takip etmek için - Brute-force koruması
/// </summary>
public class FailedLoginAttemptConfiguration : IEntityTypeConfiguration<FailedLoginAttempt>
{
    public void Configure(EntityTypeBuilder<FailedLoginAttempt> builder)
    {
        builder.ToTable("FailedLoginAttempts");
        builder.HasKey(fla => fla.Id);

        // ============ Properties ============
        builder.Property(fla => fla.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(fla => fla.UserId)
            .HasColumnName("UserId")
            .IsRequired(false)
            .HasComment("Kullanıcı ID (nullable - giriş sırasında henüz bilinmeyebilir)");

        builder.Property(fla => fla.AttemptedEmail)
            .HasColumnName("AttemptedEmail")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .IsRequired()
            .HasComment("Giriş yapılmaya çalışılan email");

        builder.Property(fla => fla.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("nvarchar(45)")
            .HasMaxLength(45)
            .IsRequired()
            .HasComment("Denemenin yapıldığı IP adresi");

        builder.Property(fla => fla.UserAgent)
            .HasColumnName("UserAgent")
            .HasColumnType("nvarchar(max)")
            .HasComment("Tarayıcı/Cihaz bilgisi");

        builder.Property(fla => fla.ErrorReason)
            .HasColumnName("ErrorReason")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(500)
            .HasComment("Hata mesajı");

        builder.Property(fla => fla.AttemptedAt)
            .HasColumnName("AttemptedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasComment("Denemenin yapıldığı zaman");

        builder.Property(fla => fla.Location)
            .HasColumnName("Location")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .HasComment("IP'den tahmin edilen konum");

        // ============ Audit Properties ============
        builder.Property(fla => fla.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(fla => fla.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("uniqueidentifier");

        builder.Property(fla => fla.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.Property(fla => fla.UpdatedBy)
            .HasColumnName("UpdatedBy")
            .HasColumnType("uniqueidentifier");

        // ============ Foreign Key ============
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(fla => fla.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_FailedLoginAttempts_Users_UserId")
            .IsRequired(false);

        // ============ Indexes ============
        builder.HasIndex(fla => fla.UserId)
            .HasDatabaseName("IX_FailedLoginAttempts_UserId")
            .IsUnique(false);

        builder.HasIndex(fla => fla.AttemptedEmail)
            .HasDatabaseName("IX_FailedLoginAttempts_AttemptedEmail");

        builder.HasIndex(fla => fla.IpAddress)
            .HasDatabaseName("IX_FailedLoginAttempts_IpAddress");

        builder.HasIndex(fla => fla.AttemptedAt)
            .HasDatabaseName("IX_FailedLoginAttempts_AttemptedAt")
            .IsDescending();

        // Composite indexes
        builder.HasIndex(fla => new { fla.IpAddress, fla.AttemptedAt })
            .HasDatabaseName("IX_FailedLoginAttempts_IpAddress_AttemptedAt")
            .IsDescending(false, true);

        builder.HasIndex(fla => new { fla.AttemptedEmail, fla.AttemptedAt })
            .HasDatabaseName("IX_FailedLoginAttempts_AttemptedEmail_AttemptedAt")
            .IsDescending(false, true);
    }
}