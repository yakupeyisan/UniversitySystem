using Academic.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Shared.Infrastructure.Persistence.Configurations.Academic;
public class ExamRoomConfiguration : IEntityTypeConfiguration<ExamRoom>
{
    public void Configure(EntityTypeBuilder<ExamRoom> builder)
    {
        builder.ToTable("ExamRooms");
        builder.HasKey(er => er.Id);
        builder.Property(er => er.RoomNumber)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(er => er.Building)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(er => er.Floor)
            .IsRequired();
        builder.Property(er => er.Capacity)
            .IsRequired();
        builder.Property(er => er.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        builder.Property(er => er.CreatedAt)
            .IsRequired();
        builder.Property(er => er.CreatedBy);
        builder.Property(er => er.UpdatedAt);
        builder.Property(er => er.UpdatedBy);
        builder.Property(er => er.RowVersion)
            .IsRowVersion();
        builder.HasIndex(er => er.RoomNumber)
            .IsUnique();
        builder.HasIndex(er => new { er.Building, er.Floor });
        builder.HasIndex(er => er.IsActive);
    }
}