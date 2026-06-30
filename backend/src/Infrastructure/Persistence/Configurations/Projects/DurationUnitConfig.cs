using Domain.Entities.Projects;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class DurationUnitConfig : IEntityTypeConfiguration<DurationUnit>
    {
        public void Configure(EntityTypeBuilder<DurationUnit> builder)
        {
            builder.ToTable("DurationUnits", "lookup");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasData(
                new DurationUnit { Id = DurationUnitsLookup.Days.Id, Name = DurationUnitsLookup.Days.Name },
                new DurationUnit { Id = DurationUnitsLookup.Weeks.Id, Name = DurationUnitsLookup.Weeks.Name },
                new DurationUnit { Id = DurationUnitsLookup.Months.Id, Name = DurationUnitsLookup.Months.Name }
            );
        }
    }
}