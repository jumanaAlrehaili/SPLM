using Domain.Entities.Features;
using Domain.Entities.Projects;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Lookups;

public class EstimationUnitConfig: IEntityTypeConfiguration<EstimationUnit>
{
    public void Configure(EntityTypeBuilder<EstimationUnit> builder)
    {
        builder.ToTable("EstimationUnits", "lookup");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(
            new EstimationUnit { Id = EstimationUnitsLookup.Hours.Id, Name = EstimationUnitsLookup.Hours.Name },
            new EstimationUnit { Id = EstimationUnitsLookup.Days.Id, Name = EstimationUnitsLookup.Days.Name }
        );
    }
}