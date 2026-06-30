using Domain.Entities.Releases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Releases
{
    public class ReleaseStageConfiguration : IEntityTypeConfiguration<ReleaseStage>
    {
        public void Configure(EntityTypeBuilder<ReleaseStage> builder)
        {
            builder.ToTable("ReleaseStages", "release");

            builder.HasKey(rs => rs.Id);
            builder.Property(rs => rs.StageName).HasMaxLength(100);

            builder.HasIndex(rs => new { rs.ReleaseId, rs.Sequence });

            builder.HasOne(rs => rs.Release)
                .WithMany(r => r.ReleaseStages)
                .HasForeignKey(rs => rs.ReleaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Stage)
                .WithMany()
                .HasForeignKey(rs => rs.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rs => rs.Status)
                .WithMany()
                .HasForeignKey(rs => rs.StatusId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasOne(rs => rs.CreatedByUser)
                .WithMany()
                .HasForeignKey(rs => rs.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rs => rs.UpdatedByUser)
                .WithMany()
                .HasForeignKey(rs => rs.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
