using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Entities.Holidays;
using Domain.Entities.Notifications;
using Domain.Entities.Projects;
using Domain.Entities.Releases;
using Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // --- Holiday Tables ---
    public DbSet<Holiday> Holidays { get; set; }

    // --- Project Tables ---
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectResource> ProjectResources { get; set; }
    public DbSet<ProjectJoinRequest> ProjectJoinRequests { get; set; }
    public DbSet<ProjectLead> ProjectLeads { get; set; }

    // --- Lookup Tables (Project) ---
    public DbSet<LeadRole> LeadRoles { get; set; }

    // --- Release Tables ---
    public DbSet<ReleasePlan> ReleasePlans { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<ReleaseStage> ReleaseStages { get; set; }
    public DbSet<ReleaseStageStatus> ReleaseStageStatuses { get; set; }
    public DbSet<ReleaseStagePrerequisite> ReleaseStagePrerequisites { get; set; }
    public DbSet<ReleaseStageHistory> ReleaseStageHistories { get; set; }

    // --- Feature Tables ---
    public DbSet<Feature> Features { get; set; }
    public DbSet<FeatureAssignment> FeatureAssignments { get; set; }
    public DbSet<FeatureStageLog> FeatureStageLogs { get; set; }

    // --- Notifications ---
    public DbSet<Notification> Notifications { get; set; }

    // --- Lookup Tables ---
    public DbSet<Stage> Stages { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<FeaturePriority> FeaturePriorities { get; set; }
    public DbSet<RequestStatus> RequestStatuses { get; set; }
    public DbSet<ReleaseStageChangeType> ReleaseStageChangeTypes { get; set; }
    public DbSet<DurationUnit> DurationUnits { get; set; }
    public DbSet<ProjectMembershipStatus> ProjectMembershipStatuses { get; set; }
    public DbSet<EstimationUnit> EstimationUnits { get; set; }


    // --- Transactions ---
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
