using Domain.Entities.Features;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Features
{
    public class StageConfiguration : IEntityTypeConfiguration<Stage>
    {
        public void Configure(EntityTypeBuilder<Stage> builder)
        {
            builder.ToTable("Stages", "lookup");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.StageName).IsRequired().HasMaxLength(50);

            builder.HasData(
                new Stage { Id = StageLookup.BA.Id, StageName = StageLookup.BA.Name, Sequence = 10, IsDefault = true },
                new Stage { Id = StageLookup.SA.Id, StageName = StageLookup.SA.Name, Sequence = 20, IsDefault = true },
                new Stage { Id = StageLookup.UIUX.Id, StageName = StageLookup.UIUX.Name, Sequence = 30, IsDefault = true },
                new Stage { Id = StageLookup.Dev.Id, StageName = StageLookup.Dev.Name, Sequence = 40, IsDefault = true },
                new Stage { Id = StageLookup.QA.Id, StageName = StageLookup.QA.Name, Sequence = 50, IsDefault = true },
                new Stage { Id = StageLookup.UAT.Id, StageName = StageLookup.UAT.Name, Sequence = 60, IsDefault = true }
            );
        }
    }
}
