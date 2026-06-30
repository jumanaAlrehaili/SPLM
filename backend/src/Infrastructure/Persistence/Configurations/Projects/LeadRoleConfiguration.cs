using Domain.Entities.Projects;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Projects
{
    public class LeadRoleConfiguration : IEntityTypeConfiguration<LeadRole>
    {
        public void Configure(EntityTypeBuilder<LeadRole> builder)
        {
            builder.ToTable("LeadRoles", "lookup");

            builder.HasKey(lr => lr.Id);
            builder.Property(lr => lr.Name).IsRequired().HasMaxLength(50);

            builder.HasOne(lr => lr.Stage)
                .WithMany()
                .HasForeignKey(lr => lr.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.AssigneeRole)
                .WithMany()
                .HasForeignKey(lr => lr.AssigneeRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                LeadRoleLookup.All.Select(lr => new LeadRole
                {
                    Id      = lr.Id,
                    Name    = lr.Name,
                    StageId = lr.StageId,
                    AssigneeRoleId = lr.AssigneeRoleId
                })
            );
        }
    }
}
