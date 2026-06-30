using Domain.Entities.Projects;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class RequestStatusConfig : IEntityTypeConfiguration<RequestStatus>
    {
        public void Configure(EntityTypeBuilder<RequestStatus> builder)
        {
            builder.ToTable("RequestStatuses", "lookup");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasData(
                new RequestStatus { Id = RequestStatusesLookup.Pending.Id, Name = RequestStatusesLookup.Pending.Name },
                new RequestStatus { Id = RequestStatusesLookup.Approved.Id, Name = RequestStatusesLookup.Approved.Name },
                new RequestStatus { Id = RequestStatusesLookup.Rejected.Id, Name = RequestStatusesLookup.Rejected.Name }
            );
        }
    }
}