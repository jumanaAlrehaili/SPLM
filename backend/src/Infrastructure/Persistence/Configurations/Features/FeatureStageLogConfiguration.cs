using Domain.Entities.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Features
{
    public class FeatureStageLogConfiguration : IEntityTypeConfiguration<FeatureStageLog>
    {
        public void Configure(EntityTypeBuilder<FeatureStageLog> builder)
        {
            builder.ToTable("FeatureStageLogs", "feature");

            builder.HasKey(l => l.Id);

            builder.HasOne(l => l.Feature)
                .WithMany(f => f.FeatureStageLogs) 
                .HasForeignKey(l => l.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(l => new { l.FeatureId, l.Timestamp });
        }
    }
}
