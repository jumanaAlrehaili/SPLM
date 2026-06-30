using Domain.Entities.Projects;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class ProjectMembershipStatusConfig : IEntityTypeConfiguration<ProjectMembershipStatus>
    {
        public void Configure(EntityTypeBuilder<ProjectMembershipStatus> builder)
        {
            builder.ToTable("ProjectMembershipStatuses", "lookup");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasData(
                new ProjectMembershipStatus { Id = ProjectMembershipStatusLookup.None.Id, Name = ProjectMembershipStatusLookup.None.Name },
                new ProjectMembershipStatus { Id = ProjectMembershipStatusLookup.Pending.Id, Name = ProjectMembershipStatusLookup.Pending.Name },
                new ProjectMembershipStatus { Id = ProjectMembershipStatusLookup.Member.Id, Name = ProjectMembershipStatusLookup.Member.Name },
                new ProjectMembershipStatus { Id = ProjectMembershipStatusLookup.Owner.Id, Name = ProjectMembershipStatusLookup.Owner.Name }
            );
        }
    }
}