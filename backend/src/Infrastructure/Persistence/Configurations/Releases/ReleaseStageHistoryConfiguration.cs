using Domain.Entities.Releases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Releases
{
    public class ReleaseStageHistoryConfiguration : IEntityTypeConfiguration<ReleaseStageHistory>
    {
        public void Configure(EntityTypeBuilder<ReleaseStageHistory> builder)
        {
            builder.ToTable("ReleaseStageHistories", "release");

            builder.HasKey(rsh => rsh.Id);

            builder.Property(rsh => rsh.ChangeType)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.Property(rsh => rsh.Notes).HasMaxLength(1000);

            builder.HasOne(rsh => rsh.ReleaseStage)
                .WithMany(rs => rs.Histories)
                .HasForeignKey(rsh => rsh.ReleaseStageId)
                .OnDelete(DeleteBehavior.Cascade);

            // NoAction to avoid multiple cascade paths through Statuses
            builder.HasOne(rsh => rsh.OldStatus)
                .WithMany()
                .HasForeignKey(rsh => rsh.OldStatusId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(rsh => rsh.NewStatus)
                .WithMany()
                .HasForeignKey(rsh => rsh.NewStatusId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(rsh => rsh.ChangedByUser)
                .WithMany()
                .HasForeignKey(rsh => rsh.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
