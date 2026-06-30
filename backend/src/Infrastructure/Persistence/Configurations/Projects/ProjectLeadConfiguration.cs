using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class ProjectLeadConfiguration : IEntityTypeConfiguration<ProjectLead>
    {
        public void Configure(EntityTypeBuilder<ProjectLead> builder)
        {
            builder.ToTable("ProjectLeads", "project");

            builder.HasKey(pl => pl.Id);

            builder.HasIndex(pl => new { pl.ProjectId, pl.LeadRoleId }).IsUnique();
            builder.HasIndex(pl => new { pl.ProjectId, pl.UserId });

            builder.HasOne(pl => pl.Project)
                .WithMany(p => p.ProjectLeads)
                .HasForeignKey(pl => pl.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pl => pl.LeadRole)
                .WithMany(lr => lr.ProjectLeads)
                .HasForeignKey(pl => pl.LeadRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pl => pl.CreatedByUser)
                .WithMany()
                .HasForeignKey(pl => pl.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pl => pl.UpdatedByUser)
                .WithMany()
                .HasForeignKey(pl => pl.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
