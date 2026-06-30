using Application.Shared.Interfaces;
using Application.Shared.DTOs.ReleasePlan;
using Domain.Entities.Features;
using Domain.Entities.Releases;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ReleasePlanService : IReleasePlanService
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public ReleasePlanService(IAppDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        // ── Release Plans ────────────────────────────────────────

        public async Task<IEnumerable<ReleasePlanOutput>> GetReleasePlansAsync(int projectId)
        {
            await VerifyProjectMemberAsync(projectId);

            return await _db.ReleasePlans
                .AsNoTracking()
                .Where(p => p.ProjectId == projectId)
                .Select(p => new ReleasePlanOutput
                {
                    Id           = p.Id,
                    Name         = p.Name,
                    Description  = p.Description,
                    ReleaseCount = p.Releases.Count(),
                    CreatedBy    = p.CreatedByUser.FullName,
                    CreatedAt    = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ReleasePlanDetailOutput?> GetReleasePlanByIdAsync(int projectId, int planId)
        {
            await VerifyProjectMemberAsync(projectId);

            return await _db.ReleasePlans
                .AsNoTracking()
                .Where(p => p.Id == planId && p.ProjectId == projectId)
                .Select(p => new ReleasePlanDetailOutput
                {
                    Id          = p.Id,
                    Name        = p.Name,
                    Description = p.Description,
                    CreatedBy   = p.CreatedByUser.FullName,
                    CreatedAt   = p.CreatedAt,
                    Releases    = p.Releases.Select(r => new ReleaseOutput
                    {
                        Id          = r.Id,
                        Name        = r.Name,
                        Description = r.Description,
                        Status      = r.Status.StatusName,
                        StartDate   = r.ReleaseStages.Any() ? r.ReleaseStages.Min(s => s.StartDate) : (DateTime?)null,
                        EndDate     = r.ReleaseStages.Any() ? r.ReleaseStages.Max(s => s.EndDate) : (DateTime?)null,
                        StageCount  = r.ReleaseStages.Count()
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ReleasePlanOutput> CreateReleasePlanAsync(int projectId, CreateReleasePlanInput input)
        {
            await VerifyPmInProjectAsync(projectId);

            var plan = new ReleasePlan
            {
                ProjectId       = projectId,
                Name            = input.Name,
                Description     = input.Description,
                CreatedAt       = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId
            };

            _db.ReleasePlans.Add(plan);
            await _db.SaveChangesAsync();

            return await _db.ReleasePlans
                .AsNoTracking()
                .Where(p => p.Id == plan.Id)
                .Select(p => new ReleasePlanOutput
                {
                    Id           = p.Id,
                    Name         = p.Name,
                    Description  = p.Description,
                    ReleaseCount = 0,
                    CreatedBy    = p.CreatedByUser.FullName,
                    CreatedAt    = p.CreatedAt
                })
                .FirstAsync();
        }

        public async Task<ReleasePlanOutput> UpdateReleasePlanAsync(int projectId, int planId, UpdateReleasePlanInput input)
        {
            await VerifyPmInProjectAsync(projectId);

            var plan = await _db.ReleasePlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.ProjectId == projectId)
                ?? throw new Exception("Release plan not found.");

            plan.Name            = input.Name;
            plan.Description     = input.Description;
            plan.UpdatedAt       = DateTime.UtcNow;
            plan.UpdatedByUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            return await _db.ReleasePlans
                .AsNoTracking()
                .Where(p => p.Id == plan.Id)
                .Select(p => new ReleasePlanOutput
                {
                    Id           = p.Id,
                    Name         = p.Name,
                    Description  = p.Description,
                    ReleaseCount = p.Releases.Count(),
                    CreatedBy    = p.CreatedByUser.FullName,
                    CreatedAt    = p.CreatedAt
                })
                .FirstAsync();
        }

        public async Task DeleteReleasePlanAsync(int projectId, int planId)
        {
            await VerifyPmInProjectAsync(projectId);

            var plan = await _db.ReleasePlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.ProjectId == projectId)
                ?? throw new Exception("Release plan not found.");

            _db.ReleasePlans.Remove(plan);
            await _db.SaveChangesAsync();
        }

        // ── Releases ─────────────────────────────────────────────

        public async Task<IEnumerable<ReleaseOutput>> GetReleasesAsync(int projectId, int planId)
        {
            await VerifyProjectMemberAsync(projectId);
            await VerifyPlanBelongsToProjectAsync(planId, projectId);

            return await _db.Releases
                .AsNoTracking()
                .Where(r => r.ReleasePlanId == planId)
                .Select(r => new ReleaseOutput
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status.StatusName,
                    StartDate = r.ReleaseStages.Any() ? r.ReleaseStages.Min(s => s.StartDate) : (DateTime?)null,
                    EndDate = r.ReleaseStages.Any() ? r.ReleaseStages.Max(s => s.EndDate) : (DateTime?)null,
                    StageCount = r.ReleaseStages.Count()
                })
                .ToListAsync();
        }
        public async Task<ReleaseDetailOutput?> GetReleaseByIdAsync(int projectId, int planId, int releaseId)
        {
            await VerifyProjectMemberAsync(projectId);
            await VerifyPlanBelongsToProjectAsync(planId, projectId);

            return await _db.Releases
                .AsNoTracking()
                .Where(r => r.Id == releaseId && r.ReleasePlanId == planId)
                .Select(r => new ReleaseDetailOutput
                {
                    Id          = r.Id,
                    Name        = r.Name,
                    Description = r.Description,
                    Status      = r.Status.StatusName,
                    StartDate   = r.ReleaseStages.Any() ? r.ReleaseStages.Min(s => s.StartDate) : (DateTime?)null,
                    EndDate     = r.ReleaseStages.Any() ? r.ReleaseStages.Max(s => s.EndDate) : (DateTime?)null,
                    StageCount  = r.ReleaseStages.Count(),
                    Stages      = r.ReleaseStages
                        .OrderBy(s => s.Sequence)
                        .Select(s => new ReleaseStageOutput
                        {
                            Id             = s.Id,
                            ReleaseId      = s.ReleaseId,
                            StageId        = s.StageId,
                            StageName      = s.StageName,
                            Sequence       = s.Sequence,
                            StartDate      = s.StartDate,
                            EndDate        = s.EndDate,
                            StatusId       = s.StatusId
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ReleaseDetailOutput> CreateReleaseAsync(int projectId, int planId, CreateReleaseInput input)
        {
            await VerifyPmInProjectAsync(projectId);
            await VerifyPlanBelongsToProjectAsync(planId, projectId);

            var nameExists = await _db.Releases
               .AnyAsync(r => r.ReleasePlanId == planId && r.Name == input.Name);

            if (nameExists)
                throw new InvalidOperationException($"A release with the name '{input.Name}' already exists in this plan.");

            var release = new Release
            {
                ReleasePlanId   = planId,
                Name            = input.Name,
                Description     = input.Description,
                StatusId        = FeatureStatusLookup.New.Id, //from statuses table 
                CreatedAt       = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId
            };

            _db.Releases.Add(release);
            await _db.SaveChangesAsync();

            // Auto-generate all default stages for the release
            var defaultStages = await _db.Stages
                .AsNoTracking()
                .Where(s => s.IsDefault)
                .OrderBy(s => s.Sequence)
                .ToListAsync();

            int sequence = 1;
            foreach (var stage in defaultStages)
            {
                if (stage.Id == StageLookup.UIUX.Id && !input.IncludeUIUXStage)
                {
                    continue;
                }

                _db.ReleaseStages.Add(new ReleaseStage
                {
                    ReleaseId       = release.Id,
                    StageId         = stage.Id,
                    StageName       = stage.StageName,
                    Sequence        = sequence++,
                    StartDate       = DateTime.UtcNow,
                    EndDate         = DateTime.UtcNow,
                    StatusId        = FeatureStatusLookup.New.Id,
                    CreatedAt       = DateTime.UtcNow,
                    CreatedByUserId = _currentUser.UserId
                });
            }

            await _db.SaveChangesAsync();

            return await GetReleaseByIdAsync(projectId, planId, release.Id)
                ?? throw new Exception("Failed to retrieve created release.");
        }

        public async Task<ReleaseDetailOutput> UpdateReleaseAsync(int projectId, int planId, int releaseId, UpdateReleaseInput input)
        {
            await VerifyPmInProjectAsync(projectId);
            await VerifyPlanBelongsToProjectAsync(planId, projectId);

            var release = await _db.Releases
                .FirstOrDefaultAsync(r => r.Id == releaseId && r.ReleasePlanId == planId)
                ?? throw new Exception("Release not found.");

            release.Name            = input.Name;
            release.Description     = input.Description;
            release.UpdatedAt       = DateTime.UtcNow;
            release.UpdatedByUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            return await GetReleaseByIdAsync(projectId, planId, release.Id)
                ?? throw new Exception("Failed to retrieve updated release.");
        }

        public async Task DeleteReleaseAsync(int projectId, int planId, int releaseId)
        {
            await VerifyPmInProjectAsync(projectId);
            await VerifyPlanBelongsToProjectAsync(planId, projectId);

            var release = await _db.Releases
                .FirstOrDefaultAsync(r => r.Id == releaseId && r.ReleasePlanId == planId)
                ?? throw new Exception("Release not found.");

            _db.Releases.Remove(release);
            await _db.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────

        private async Task VerifyProjectMemberAsync(int projectId)
        {
            var isMember = await _db.ProjectResources
                .AnyAsync(r => r.ProjectId == projectId && r.UserId == _currentUser.UserId);

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this project.");
        }

        private async Task VerifyPmInProjectAsync(int projectId)
        {
            var isPM = await _db.ProjectResources
                .AnyAsync(r => r.ProjectId == projectId
                            && r.UserId == _currentUser.UserId
                            && r.RoleId == RoleLookup.ProjectManager.Id);

            if (!isPM)
                throw new UnauthorizedAccessException("Only the Project Manager can perform this action.");
        }

        private async Task VerifyPlanBelongsToProjectAsync(int planId, int projectId)
        {
            var belongs = await _db.ReleasePlans
                .AnyAsync(p => p.Id == planId && p.ProjectId == projectId);

            if (!belongs)
                throw new Exception("Release plan not found in this project.");
        }
    }
}
