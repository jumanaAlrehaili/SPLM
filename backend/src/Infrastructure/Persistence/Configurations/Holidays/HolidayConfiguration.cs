using Domain.Entities.Holidays;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Holidays
{
    public class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
    {
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.ToTable("Holidays", "admin");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(h => h.StartDate)
                   .IsRequired();

            builder.Property(h => h.EndDate)
                   .IsRequired();

            builder.Property(h => h.Year)
                   .IsRequired();

            builder.HasIndex(h => h.Year);

            builder.HasIndex(h => new { h.StartDate, h.EndDate });
        }
    }
}
