using Domain.Entities.Releases;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Releases
{
    public class ReleaseStageStatusConfiguration : IEntityTypeConfiguration<ReleaseStageStatus>
    {
        public void Configure(EntityTypeBuilder<ReleaseStageStatus> builder)
        {
            builder.ToTable("ReleaseStageStatuses", "lookup");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.StatusName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new ReleaseStageStatus { Id = ReleaseStageStatusLookup.NotStarted.Id, StatusName = ReleaseStageStatusLookup.NotStarted.Name },
                new ReleaseStageStatus { Id = ReleaseStageStatusLookup.InProgress.Id, StatusName = ReleaseStageStatusLookup.InProgress.Name },
                new ReleaseStageStatus { Id = ReleaseStageStatusLookup.Completed.Id, StatusName = ReleaseStageStatusLookup.Completed.Name }
            );
        }
    }
}
