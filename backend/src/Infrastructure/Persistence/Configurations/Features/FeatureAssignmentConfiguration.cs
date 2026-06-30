using Domain.Entities.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Features
{
    public class FeatureAssignmentConfiguration : IEntityTypeConfiguration<FeatureAssignment>
    {
        public void Configure(EntityTypeBuilder<FeatureAssignment> builder)
        {
            builder.ToTable("FeatureAssignments", "feature");

            builder.HasKey(fa => fa.Id);

            builder.HasIndex(fa => new { fa.FeatureId, fa.StageId }).IsUnique();

            builder.HasOne(fa => fa.Feature)
                .WithMany(f => f.Assignments)
                .HasForeignKey(fa => fa.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fa => fa.Stage)
                .WithMany()
                .HasForeignKey(fa => fa.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fa => fa.AssignedUser)
                .WithMany()
                .HasForeignKey(fa => fa.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fa => fa.CompletedByUser)
                .WithMany()
                .HasForeignKey(fa => fa.CompletedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fa => fa.CreatedByUser)
                .WithMany()
                .HasForeignKey(fa => fa.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fa => fa.UpdatedByUser)
                .WithMany()
                .HasForeignKey(fa => fa.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Estimation).IsRequired(false);

            builder.Property(x => x.StartedAt).IsRequired(false);

            builder.HasOne(x => x.EstimationUnit)
                .WithMany()
                .HasForeignKey(x => x.EstimationUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
