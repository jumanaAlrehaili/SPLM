using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class ProjectResourceConfiguration : IEntityTypeConfiguration<ProjectResource>
    {
        public void Configure(EntityTypeBuilder<ProjectResource> builder)
        {
            builder.ToTable("ProjectResources", "project");

            builder.HasKey(pr => pr.Id);

            builder.HasOne(pr => pr.Project)
                .WithMany(p => p.ProjectResources)
                .HasForeignKey(pr => pr.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict); // User cannot be deleted if they are still an active member of a project.

           
            builder.HasOne(pr => pr.CreatedByUser)
                .WithMany()
                .HasForeignKey(pr => pr.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents deleting the creator user.

            builder.HasIndex(pr => new { pr.ProjectId, pr.UserId })
                   .IsUnique();
        }
    }
}
