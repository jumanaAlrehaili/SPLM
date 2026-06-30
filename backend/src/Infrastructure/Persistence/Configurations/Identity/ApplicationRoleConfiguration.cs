using Domain.Lookups;
using Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles", "identity");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(r => r.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.HasData(
            //new ApplicationRole { Id = 1, Name = "Project Manager",   NormalizedName = "PROJECT MANAGER",       ConcurrencyStamp = "a1b2c3d4-0001-0000-0000-000000000001" },
            //new ApplicationRole { Id = 2, Name = "Business Analyst",  NormalizedName = "BUSINESS ANALYST", ConcurrencyStamp = "a1b2c3d4-0002-0000-0000-000000000002" },
            //new ApplicationRole { Id = 3, Name = "System Analyst",    NormalizedName = "SYSTEM ANALYST", ConcurrencyStamp = "a1b2c3d4-0003-0000-0000-000000000003" },
            //new ApplicationRole { Id = 4, Name = "UI/UX Designer",    NormalizedName = "UI/UX DESIGNER", ConcurrencyStamp = "a1b2c3d4-0004-0000-0000-000000000004" },
            //new ApplicationRole { Id = 5, Name = "Developer",         NormalizedName = "DEVELOPER",         ConcurrencyStamp = "a1b2c3d4-0005-0000-0000-000000000005" },
            //new ApplicationRole { Id = 6, Name = "Quality Assurance", NormalizedName = "QUALITY ASSURANCE", ConcurrencyStamp = "a1b2c3d4-0006-0000-0000-000000000006" }

            new ApplicationRole { Id = RoleLookup.ProjectManager.Id, Name = RoleLookup.ProjectManager.Name, NormalizedName = "PROJECT MANAGER", ConcurrencyStamp = "a1b2c3d4-0001-0000-0000-000000000001" },
            new ApplicationRole { Id = RoleLookup.BusinessAnalyst.Id, Name = RoleLookup.BusinessAnalyst.Name, NormalizedName = "BUSINESS ANALYST", ConcurrencyStamp = "a1b2c3d4-0002-0000-0000-000000000002" },
            new ApplicationRole { Id = RoleLookup.SystemAnalyst.Id, Name = RoleLookup.SystemAnalyst.Name, NormalizedName = "SYSTEM ANALYST", ConcurrencyStamp = "a1b2c3d4-0003-0000-0000-000000000003" },
            new ApplicationRole { Id = RoleLookup.UIUXDesigner.Id, Name = RoleLookup.UIUXDesigner.Name, NormalizedName = "UI/UX DESIGNER", ConcurrencyStamp = "a1b2c3d4-0004-0000-0000-000000000004" },
            new ApplicationRole { Id = RoleLookup.Developer.Id, Name = RoleLookup.Developer.Name, NormalizedName = "DEVELOPER", ConcurrencyStamp = "a1b2c3d4-0005-0000-0000-000000000005" },
            new ApplicationRole { Id = RoleLookup.QualityAssurance.Id, Name = RoleLookup.QualityAssurance.Name, NormalizedName = "QUALITY ASSURANCE", ConcurrencyStamp = "a1b2c3d4-0006-0000-0000-000000000006" },
            new ApplicationRole { Id = RoleLookup.Admin.Id, Name = RoleLookup.Admin.Name, NormalizedName = "ADMIN", ConcurrencyStamp = "a1b2c3d4-0007-0000-0000-000000000007" }
        );
    }
}
