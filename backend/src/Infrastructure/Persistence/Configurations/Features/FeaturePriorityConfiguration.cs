using Domain.Entities.Features;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Features
{
    public class FeaturePriorityConfiguration : IEntityTypeConfiguration<FeaturePriority>
    {
        public void Configure(EntityTypeBuilder<FeaturePriority> builder)
        {
            builder.ToTable("FeaturePriorities", "lookup");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(50);

            builder.HasData(
                new FeaturePriority { Id = FeaturePriorityLookup.Low.Id, Name = FeaturePriorityLookup.Low.Name },
                new FeaturePriority { Id = FeaturePriorityLookup.Medium.Id, Name = FeaturePriorityLookup.Medium.Name },
                new FeaturePriority { Id = FeaturePriorityLookup.High.Id, Name = FeaturePriorityLookup.High.Name },
                new FeaturePriority { Id = FeaturePriorityLookup.Critical.Id, Name = FeaturePriorityLookup.Critical.Name }
            );
        }
    }
}
