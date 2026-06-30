using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class ProjectJoinRequestConfiguration : IEntityTypeConfiguration<ProjectJoinRequest>
    {
        public void Configure(EntityTypeBuilder<ProjectJoinRequest> builder)
        {
            builder.ToTable("ProjectJoinRequests", "project");

            builder.HasKey(pjr => pjr.Id);
            builder.Property(pjr => pjr.RoleId).IsRequired();
            builder.Property(pjr => pjr.JoinReason).HasMaxLength(500);

            builder.Property(pjr => pjr.StatusId)
                   .IsRequired();

            builder.HasOne(pjr => pjr.Status)
                   .WithMany() 
                   .HasForeignKey(pjr => pjr.StatusId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pjr => pjr.Project)
                .WithMany(p => p.ProjectJoinRequests)
                .HasForeignKey(pjr => pjr.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pjr => pjr.User)
                .WithMany(u => u.JoinRequests)
                .HasForeignKey(pjr => pjr.UserId)
                .OnDelete(DeleteBehavior.Restrict); //Prevent deleting a User if they have existing join requests

            // GetAll-GetById-Search-SubmitJoinRequest
            builder.HasIndex(x => new { x.ProjectId, x.UserId, x.StatusId });
            // GetMyJoinRequests
            builder.HasIndex(x => new { x.UserId, x.CreatedAt });
        }
    }
}
