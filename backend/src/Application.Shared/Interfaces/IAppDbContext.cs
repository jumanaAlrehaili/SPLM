using Application.Shared.DTOs.Notification;
using Domain.Entities.Features;
using Domain.Entities.Holidays;
using Domain.Entities.Notifications;
using Domain.Entities.Projects;
using Domain.Entities.Releases;
using Domain.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Shared.Interfaces
{
    public interface IAppDbContext
    {
        // Identity Tables
        DbSet<ApplicationUser> Users { get; }
        DbSet<ApplicationRole> Roles { get; }

        // Holiday Tables
        DbSet<Holiday> Holidays { get; }

        // Project Tables
        DbSet<Project> Projects { get; }
        DbSet<ProjectResource> ProjectResources { get; }
        DbSet<ProjectJoinRequest> ProjectJoinRequests { get; }
        DbSet<ProjectLead> ProjectLeads { get; }

        // Lookup Tables (Project)
        DbSet<LeadRole> LeadRoles { get; }

        // Release Tables
        DbSet<ReleasePlan> ReleasePlans { get; }
        DbSet<Release> Releases { get; }
        DbSet<ReleaseStage> ReleaseStages { get; }
        DbSet<ReleaseStageStatus> ReleaseStageStatuses { get; }
        DbSet<ReleaseStagePrerequisite> ReleaseStagePrerequisites { get; }
        DbSet<ReleaseStageHistory> ReleaseStageHistories { get; }

        // Feature Tables
        DbSet<Feature> Features { get; }
        DbSet<FeatureAssignment> FeatureAssignments { get; }
        DbSet<FeatureStageLog> FeatureStageLogs { get; }
        DbSet<Notification> Notifications { get; }

        // Lookup Tables
        DbSet<Stage> Stages { get; }
        DbSet<Status> Statuses { get; }
        DbSet<FeaturePriority> FeaturePriorities { get; }
        DbSet<RequestStatus> RequestStatuses { get; set; }
        DbSet<ReleaseStageChangeType> ReleaseStageChangeTypes { get; set; }
        DbSet<DurationUnit> DurationUnits { get; set; }
        DbSet<ProjectMembershipStatus> ProjectMembershipStatuses { get; set; }
        DbSet<EstimationUnit> EstimationUnits { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
