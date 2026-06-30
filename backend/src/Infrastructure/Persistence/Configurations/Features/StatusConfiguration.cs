using Domain.Entities.Features;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Features
{
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("Statuses", "lookup");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.StatusName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new Status { Id = FeatureStatusLookup.New.Id, StatusName = FeatureStatusLookup.New.Name },
                new Status { Id = FeatureStatusLookup.InProgress.Id, StatusName = FeatureStatusLookup.InProgress.Name },
                new Status { Id = FeatureStatusLookup.PendingReview.Id, StatusName = FeatureStatusLookup.PendingReview.Name },
                new Status { Id = FeatureStatusLookup.Rejected.Id, StatusName = FeatureStatusLookup.Rejected.Name },
                new Status { Id = FeatureStatusLookup.Completed.Id, StatusName = FeatureStatusLookup.Completed.Name }
            );
        }
    }
}
