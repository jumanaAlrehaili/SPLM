using Domain.Entities.Releases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Releases
{
    public class ReleaseStagePrerequisiteConfiguration : IEntityTypeConfiguration<ReleaseStagePrerequisite>
    {
        public void Configure(EntityTypeBuilder<ReleaseStagePrerequisite> builder)
        {
            builder.ToTable("ReleaseStagePrerequisites", "release");

            builder.HasKey(rsp => rsp.Id);

            builder.HasIndex(rsp => new { rsp.ReleaseStageId, rsp.PrerequisiteReleaseStageId }).IsUnique();

            builder.HasOne(rsp => rsp.ReleaseStage)
                .WithMany(rs => rs.Prerequisites)
                .HasForeignKey(rsp => rsp.ReleaseStageId)
                .OnDelete(DeleteBehavior.Cascade);

            // NoAction to avoid multiple cascade paths from ReleaseStages
            builder.HasOne(rsp => rsp.PrerequisiteReleaseStage)
                .WithMany()
                .HasForeignKey(rsp => rsp.PrerequisiteReleaseStageId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
