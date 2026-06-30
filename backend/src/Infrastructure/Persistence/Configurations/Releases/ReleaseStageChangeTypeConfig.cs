using Domain.Entities.Releases;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Releases
{
    public class ReleaseStageChangeTypeConfig : IEntityTypeConfiguration<ReleaseStageChangeType>
    {
        public void Configure(EntityTypeBuilder<ReleaseStageChangeType> builder)
        {
            builder.ToTable("ReleaseStageChangeTypes", "lookup");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasData(
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Created.Id, Name = ReleaseStageChangeTypeLookup.Created.Name },
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Updated.Id, Name = ReleaseStageChangeTypeLookup.Updated.Name },
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Started.Id, Name = ReleaseStageChangeTypeLookup.Started.Name },
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Completed.Id, Name = ReleaseStageChangeTypeLookup.Completed.Name },
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Delayed.Id, Name = ReleaseStageChangeTypeLookup.Delayed.Name },
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Reopened.Id, Name = ReleaseStageChangeTypeLookup.Reopened.Name },
                new ReleaseStageChangeType { Id = ReleaseStageChangeTypeLookup.Cancelled.Id, Name = ReleaseStageChangeTypeLookup.Cancelled.Name }
            );
        }
    }
}