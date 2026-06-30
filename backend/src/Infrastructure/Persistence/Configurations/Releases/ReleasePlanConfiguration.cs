using Domain.Entities.Releases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Releases
{
    public class ReleasePlanConfiguration : IEntityTypeConfiguration<ReleasePlan>
    {
        public void Configure(EntityTypeBuilder<ReleasePlan> builder)
        {
            builder.ToTable("ReleasePlans", "release");

            builder.HasKey(rp => rp.Id);
            builder.Property(rp => rp.Name).IsRequired().HasMaxLength(150);
            builder.Property(rp => rp.Description).HasMaxLength(1000);

            builder.HasOne(rp => rp.Project)
                .WithMany(p => p.ReleasePlans)
                .HasForeignKey(rp => rp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.CreatedByUser)
                .WithMany()
                .HasForeignKey(rp => rp.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rp => rp.UpdatedByUser)
                .WithMany()
                .HasForeignKey(rp => rp.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.ProjectId, p.Name }).IsUnique();
        }
    }
}
