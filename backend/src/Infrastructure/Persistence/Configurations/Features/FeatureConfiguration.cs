using Domain.Entities.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Features
{
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable("Features", "feature");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Title).IsRequired().HasMaxLength(200);

            builder.HasOne(f => f.Priority)
                .WithMany()
                .HasForeignKey(f => f.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(f => f.UpdatedAt).IsRequired(false);

            builder.HasOne(f => f.CurrentStatus)
                .WithMany()
                .HasForeignKey(f => f.CurrentStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Project)
                .WithMany(p => p.Features)
                .HasForeignKey(f => f.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Release)
                .WithMany(r => r.Features)
                .HasForeignKey(f => f.ReleaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.CreatedByUser)
                .WithMany()
                .HasForeignKey(f => f.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.UpdatedByUser)
                .WithMany()
                .HasForeignKey(f => f.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.CreatedAt });
        }
    }
}
