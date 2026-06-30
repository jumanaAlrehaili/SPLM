using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using System.Net.NetworkInformation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Services
{
    public class FeatureService : IFeatureService
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public FeatureService(IAppDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<FeatureOutput>> GetAllFeaturesAsync(int projectId)
        {
            await VerifyProjectMemberAsync(projectId);

            return await _db.Features
                .AsNoTracking()
                .Where(f => f.ProjectId == projectId)
                .Select(f => new FeatureOutput
                {
                    Id          = f.Id,
                    Title       = f.Title,
                    Description = f.Description,
                    EpicLink    = f.EpicLink,
                    PriorityId  = f.PriorityId,
                    CompletedAt = f.CompletedAt,
                    Status      = f.CurrentStatus.StatusName,
                    Release     = f.Release == null ? null : new FeatureReleaseDto
                    {
                        Id   = f.Release.Id,
                        Name = f.Release.Name
                    },
                    CreatedBy = f.CreatedByUser.FullName,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FeatureOutput>> SearchFeaturesAsync(int projectId, string? name, int? priority, int? statusId)
        {
            await VerifyProjectMemberAsync(projectId);

            return await _db.Features
                .AsNoTracking()
                .Where(f => f.ProjectId == projectId)
                .Where(f => string.IsNullOrEmpty(name) ||
                            f.Title.Contains(name))
                .Where(f => priority == null || f.PriorityId == priority)
                .Where(f => statusId == null || f.CurrentStatusId == statusId)
                .Select(f => new FeatureOutput
                {
                    Id          = f.Id,
                    Title       = f.Title,
                    Description = f.Description,
                    EpicLink    = f.EpicLink,
                    PriorityId  = f.PriorityId,
                    CompletedAt = f.CompletedAt,
                    Status      = f.CurrentStatus.StatusName,
                    Release     = f.Release == null ? null : new FeatureReleaseDto
                    {
                        Id   = f.Release.Id,
                        Name = f.Release.Name
                    },
                    CreatedBy = f.CreatedByUser.FullName,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<FeatureDetailOutput?> GetFeatureByIdAsync(int projectId, int featureId)
        {
            // Verify that the user has access to the project
            await VerifyProjectMemberAsync(projectId);

            var feature = await _db.Features
                .AsNoTracking()
                .Where(f => f.Id == featureId && f.ProjectId == projectId)
                .Select(f => new
                {
                    f.Id,
                    f.Title,
                    f.Description,
                    f.EpicLink,
                    f.PriorityId,
                    f.CompletedAt,
                    f.CreatedAt,
                    StatusName = f.CurrentStatus.StatusName,
                    CreatedBy = f.CreatedByUser.FullName,
                    Release = f.Release == null ? null : new { f.Release.Id, f.Release.Name }
                })
                .FirstOrDefaultAsync();

            if (feature == null) return null;

            List<int>? activeStageIds = null;
            if (feature.Release != null)
            {
                activeStageIds = await _db.ReleaseStages
                    .Where(rs => rs.ReleaseId == feature.Release.Id)
                    .Select(rs => rs.StageId)
                    .ToListAsync();
            }

            // Get all available stages and project them with existing assignments, this ensures all stages appear in the UI even if unassigned
            var stagesQuery = _db.Stages.AsNoTracking();
            if (activeStageIds != null)
            {
                stagesQuery = stagesQuery.Where(s => activeStageIds.Contains(s.Id));
            }

            // Get the assigned user for this specific stage and feature
            var existingAssignments = await _db.FeatureAssignments
                .Where(fa => fa.FeatureId == featureId)
                .Select(fa => new
                {
                  fa.StageId,
                  fa.AssignedUserId,
                  fa.AssignedUser.FullName,
                  fa.CompletedAt
                })
               .ToListAsync();

            var assignments = stagesQuery
                .OrderBy(s => s.Sequence)
                .AsEnumerable()
                .Select(stage => 
                {
                  var assignment = existingAssignments.FirstOrDefault(fa => fa.StageId == stage.Id);
                 
                    return new FeatureAssignmentDto
                  {
                    StageId = stage.Id,
                    StageName = stage.StageName,
                    UserId = assignment?.AssignedUserId,
                    UserName = assignment?.FullName,
                     CompletedAt = assignment?.CompletedAt
                  };
                })
               .ToList();

        var permissions = await GetUserPermissionsForFeature(projectId);

            // Construct the detailed output
            return new FeatureDetailOutput
            {
                Id = feature.Id,
                Title = feature.Title,
                Description = feature.Description,
                EpicLink = feature.EpicLink,
                PriorityId = feature.PriorityId,
                CompletedAt = feature.CompletedAt,
                Status = feature.StatusName,
                Release = feature.Release == null ? null : new FeatureReleaseDto
                {
                    Id = feature.Release.Id,
                    Name = feature.Release.Name
                },
                CreatedBy = feature.CreatedBy,
                CreatedAt = feature.CreatedAt,
                // Assign the processed list containing all stages
                Assignments = assignments,
                Permissions = permissions
            };
        }

        public async Task<FeatureDetailOutput> CreateFeatureAsync(int projectId, CreateFeatureInput input)
        {
            await VerifyBaInProjectAsync(projectId);

            var feature = new Feature
            {
                Title           = input.Title,
                Description     = input.Description,
                EpicLink        = input.EpicLink,
                PriorityId      = input.Priority,
                CurrentStatusId = FeatureStatusLookup.New.Id,
                ProjectId       = projectId,
                CreatedAt       = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId
            };

            _db.Features.Add(feature);
            await _db.SaveChangesAsync();

            return await GetFeatureByIdAsync(projectId, feature.Id)
                ?? throw new Exception("Failed to retrieve created feature.");
        }

        public async Task<FeatureDetailOutput> UpdateFeatureAsync(int projectId, int featureId, UpdateFeatureInput input)
        {
            await VerifyBaInProjectAsync(projectId);

            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && f.ProjectId == projectId)
                ?? throw new Exception("Feature not found.");

            feature.Title           = input.Title;
            feature.Description     = input.Description;
            feature.EpicLink        = input.EpicLink;
            feature.PriorityId      = input.Priority;
            feature.UpdatedAt       = DateTime.UtcNow;
            feature.UpdatedByUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            return await GetFeatureByIdAsync(projectId, feature.Id)
                ?? throw new Exception("Failed to retrieve updated feature.");
        }

        public async Task DeleteFeatureAsync(int projectId, int featureId)
        {
            await VerifyBaInProjectAsync(projectId);

            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && f.ProjectId == projectId)
                ?? throw new Exception("Feature not found.");

            _db.Features.Remove(feature);
            await _db.SaveChangesAsync();
        }

        public async Task AssignUserToStageAsync(int projectId, int featureId, int stageId, int assignedUserId)
        {
            var isPM = await _db.ProjectResources
                .AnyAsync(r => r.ProjectId == projectId
                    && r.UserId == _currentUser.UserId
                    && r.RoleId == RoleLookup.ProjectManager.Id);

            // If not a PM, ensure the user is the assigned Lead for this stage
            if (!isPM)
            {
                await VerifyUserisLeadForStageAsync(projectId, stageId);
            }

            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && f.ProjectId == projectId)
                ?? throw new KeyNotFoundException("Feature not found in this project.");

            if (feature.ReleaseId.HasValue)
            {
                var stageExistsInRelease = await _db.ReleaseStages
                    .AnyAsync(rs => rs.ReleaseId == feature.ReleaseId && rs.StageId == stageId);

                if (!stageExistsInRelease)
                {
                    throw new InvalidOperationException("This stage is not included in the feature's release.");
                }
            }

            var leadRoleConfig = await _db.LeadRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(lr => lr.StageId == stageId)
                ?? throw new KeyNotFoundException("No configuration found for this stage.");

            var isValidResource = await _db.ProjectResources
                .AnyAsync(pr => pr.ProjectId == projectId
                               && pr.UserId == assignedUserId
                               && pr.RoleId == leadRoleConfig.AssigneeRoleId); 

            if (!isValidResource)
            {
                throw new InvalidOperationException("This user is not registered in the project or doesn't have the required role");
            }

            var assignment = await _db.FeatureAssignments
                .FirstOrDefaultAsync(fa => fa.FeatureId == featureId && fa.StageId == stageId);

            if (assignment != null && assignment.CompletedAt.HasValue)
            {
                throw new InvalidOperationException("Cannot change the assigned user for an already completed stage.");
            }

            if (assignment == null)
            {   
                //insert new assignment 
                assignment = new FeatureAssignment
                {
                    FeatureId = featureId,
                    StageId = stageId,
                    AssignedUserId = assignedUserId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUser.UserId
                };
                _db.FeatureAssignments.Add(assignment);
            }
            else
            {
                // Update the assigned user 
                assignment.AssignedUserId = assignedUserId;
                assignment.UpdatedAt = DateTime.UtcNow;
                assignment.UpdatedByUserId = _currentUser.UserId;
            }

            await _db.SaveChangesAsync();
        }
        public async Task UnassignUserFromStageAsync(int projectId, int featureId, int stageId)
        {
            var isPM = await _db.ProjectResources
                .AnyAsync(r => r.ProjectId == projectId
                    && r.UserId == _currentUser.UserId
                    && r.RoleId == RoleLookup.ProjectManager.Id);

            // If not a PM, ensure the user is the assigned Lead for this stage
            if (!isPM)
            {
                await VerifyUserisLeadForStageAsync(projectId, stageId);
            }

            var assignment = await _db.FeatureAssignments
                .FirstOrDefaultAsync(fa => fa.FeatureId == featureId && fa.StageId == stageId)
                ?? throw new KeyNotFoundException("No assignment found for this feature in the specified stage.");

            _db.FeatureAssignments.Remove(assignment);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AvailableStageResourceDto>> GetAvailableResourcesForStageAsync(int projectId, int stageId)
        {
            await VerifyProjectMemberAsync(projectId);

            var leadRoleConfig = await _db.LeadRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(lr => lr.StageId == stageId)
                ?? throw new KeyNotFoundException("No configuration found for this stage.");

            return await _db.ProjectResources
                .AsNoTracking()
                .Where(pr => pr.ProjectId == projectId && pr.RoleId == leadRoleConfig.AssigneeRoleId)
                .Select(pr => new AvailableStageResourceDto(pr.UserId, pr.User.FullName))
                .ToListAsync();
        }

        public async Task MoveToNextStageInFeatureAsync(int projectId, int featureId, int currentStageId)
        {
            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && f.ProjectId == projectId)
                ?? throw new KeyNotFoundException("Feature not found in this project.");

            var currentAssignment = await _db.FeatureAssignments
                .FirstOrDefaultAsync(fa => fa.FeatureId == featureId
                                        && fa.StageId == currentStageId
                                        && fa.AssignedUserId == _currentUser.UserId)
                ?? throw new UnauthorizedAccessException("You are not the assigned user responsible for completing this stage.");

            var currentStageSequence = await _db.Stages
                .Where(s => s.Id == currentStageId)
                .Select(s => s.Sequence)
                .FirstOrDefaultAsync();

            if (feature.ReleaseId.HasValue)
            {
                // Get all stages within this release that must precede the current stage
                // Example: If current stage is Dev (Sequence: 3), required stages will be BA (Sequence: 1) and SA (Sequence: 2).
                var requiredStageIds = await _db.ReleaseStages 
                    .Where(rs => rs.ReleaseId == feature.ReleaseId && rs.Stage.Sequence < currentStageSequence) 
                    .Select(rs => rs.StageId)
                    .ToListAsync();

                if (requiredStageIds.Any())
                {
                    // Count how many of these preceding stages have been fully completed for this feature.
                    var completedStagesCount = await _db.FeatureAssignments
                        .CountAsync(fa => fa.FeatureId == featureId
                                       && requiredStageIds.Contains(fa.StageId) //ex: check only the stages 1,2 and ignore qa(4).
                                       && fa.CompletedAt != null);

                    if (completedStagesCount < requiredStageIds.Count)
                    {
                        throw new InvalidOperationException("Cannot complete this stage. All previous stages in the release lifecycle must be completed first.");
                    }
                }
            }

            currentAssignment.CompletedByUserId = _currentUser.UserId;
            currentAssignment.CompletedAt = DateTime.UtcNow;

            _db.FeatureStageLogs.Add(new FeatureStageLog
            {
                FeatureId = featureId,
                StageId = currentStageId,
                UserId = _currentUser.UserId,
                Timestamp = DateTime.UtcNow,
                Action = StageActions.Completed,
                Comment = "Stage completed successfully"
            });

            // Update the global feature status
            if (currentStageId == StageLookup.BA.Id || currentStageId == StageLookup.SA.Id || currentStageId == StageLookup.UIUX.Id)
            {
                feature.CurrentStatusId = FeatureStatusLookup.InProgress.Id;
            }
            else if (currentStageId == StageLookup.Dev.Id || currentStageId == StageLookup.QA.Id)
            {
                feature.CurrentStatusId = FeatureStatusLookup.PendingReview.Id;
            }
            else if (currentStageId == StageLookup.UAT.Id)
            {
                feature.CurrentStatusId = FeatureStatusLookup.Completed.Id;
                feature.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Invalid workflow stage execution pattern.");
            }

            feature.UpdatedAt = DateTime.UtcNow;
            feature.UpdatedByUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();
        }

        public async Task RejectAndMoveToPreviousStageAsync(int projectId, int featureId, int currentStageId, string rejectionComment)
        {
            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && f.ProjectId == projectId)
                ?? throw new KeyNotFoundException("Feature not found in this project.");
            
            var currentAssignment = await _db.FeatureAssignments
                .FirstOrDefaultAsync(fa => fa.FeatureId == featureId && fa.StageId == currentStageId);

            if (currentAssignment == null || currentAssignment.AssignedUserId != _currentUser.UserId)
            {
                throw new UnauthorizedAccessException("You are not the assigned user responsible for rejecting this stage.");
            }

            // Record the rejection in logs
            _db.FeatureStageLogs.Add(new FeatureStageLog             
            {                                                        
                FeatureId = featureId,                               
                StageId = currentStageId,                           
                UserId = _currentUser.UserId,                        
                Timestamp = DateTime.UtcNow,                        
                Action = StageActions.Rejected,                      
                Comment = rejectionComment                           
            });

            // Retrieve sequence for both Dev stage and the current rejecting stage
            var workflowSequences = await _db.Stages
                .AsNoTracking()
                .Where(s => s.Id == currentStageId || s.Id == StageLookup.Dev.Id) //CurrentStageId = Rejected stage (UAT or QA).
                .Select(s => new { s.Id, s.Sequence })
                .ToListAsync();

            var rejectionStageSequence = workflowSequences.FirstOrDefault(s => s.Id == currentStageId)?.Sequence; // = 5(QA) or 6(UAT).
            var developmentStageSequence = workflowSequences.FirstOrDefault(s => s.Id == StageLookup.Dev.Id)?.Sequence; // = 4(Dev)

            // Filter assignments to get only the stages involved in the rejection process
            var affectedAssignments = await _db.FeatureAssignments
                .Where(fa => fa.FeatureId == featureId
                                          && _db.Stages.Any(s => s.Id == fa.StageId
                                                         && s.Sequence >= developmentStageSequence
                                                         && s.Sequence <= rejectionStageSequence))
                .ToListAsync();

            foreach (var assignment in affectedAssignments)
            {
                assignment.CompletedAt = null;
                assignment.CompletedByUserId = null;
                assignment.UpdatedAt = DateTime.UtcNow;
                assignment.UpdatedByUserId = _currentUser.UserId;
            }

            feature.CurrentStatusId = FeatureStatusLookup.InProgress.Id;

            feature.UpdatedAt = DateTime.UtcNow;
            feature.UpdatedByUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<FeatureStageLogOutput>> GetFeatureLogsAsync(int projectId, int featureId)
        {
            await VerifyProjectMemberAsync(projectId);

            return await _db.FeatureStageLogs
                .AsNoTracking()
                .Where(l => l.FeatureId == featureId)
                .OrderByDescending(l => l.Timestamp) 
                .Select(l => new FeatureStageLogOutput
                {
                    Id = l.Id,
                    Action = l.Action,
                    Comment = l.Comment,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserName = l.User.FullName, 
                    StageName = l.Stage.StageName
                })
                .ToListAsync();
        }

        public async Task<FeatureDetailOutput> AssignFeatureToReleaseAsync(int projectId, int featureId, int? releaseId)
        {
            await VerifyPmInProjectAsync(projectId);

            var feature = await _db.Features
                .FirstOrDefaultAsync(f => f.Id == featureId && f.ProjectId == projectId)
                ?? throw new Exception("Feature not found.");

            if (releaseId.HasValue)
            {
                var releaseBelongs = await _db.Releases
                    .AnyAsync(r => r.Id == releaseId && r.ReleasePlan.ProjectId == projectId);

                if (!releaseBelongs)
                    throw new Exception("The specified release does not belong to this project.");

                // If the target release does not include certain stages, remove those stage assignments for this feature
                var releaseStageIds = await _db.ReleaseStages
                    .Where(rs => rs.ReleaseId == releaseId)
                    .Select(rs => rs.StageId)
                    .ToListAsync();

                var assignmentsToRemove = await _db.FeatureAssignments
                    .Where(fa => fa.FeatureId == featureId && !releaseStageIds.Contains(fa.StageId))
                    .ToListAsync();

                if (assignmentsToRemove.Any())
                {
                    _db.FeatureAssignments.RemoveRange(assignmentsToRemove);
                }
            }

            feature.ReleaseId       = releaseId;
            feature.UpdatedAt       = DateTime.UtcNow;
            feature.UpdatedByUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            return await GetFeatureByIdAsync(projectId, feature.Id)
                ?? throw new Exception("Failed to retrieve updated feature.");
        }

        // ----- Helpers -------------------- 

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
                            && r.UserId  == _currentUser.UserId
                            && r.RoleId  == RoleLookup.ProjectManager.Id);

            if (!isPM)
                throw new UnauthorizedAccessException("Only a Project Manager of this project can perform this action.");
        }

        private async Task VerifyBaInProjectAsync(int projectId)
        {
            var isBA = await _db.ProjectResources
                .AnyAsync(r => r.ProjectId == projectId
                            && r.UserId  == _currentUser.UserId
                            && r.RoleId  == RoleLookup.BusinessAnalyst.Id);

            if (!isBA)
                throw new UnauthorizedAccessException("Only a Business Analyst of this project can perform this action.");
        }

        private async Task VerifyUserisLeadForStageAsync(int projectId, int stageId)
        {
            var requiredLeadRole = await _db.LeadRoles
                .AsNoTracking()
                .FirstOrDefaultAsync(lr => lr.StageId == stageId); //stageID = 1, then LeadRoleId = 1 (BA Lead)

            if (requiredLeadRole == null)
            {
                throw new Exception("No leadership role configured for this Stage.");
            }

            var isLead = await _db.ProjectLeads
                .AnyAsync(pl => pl.ProjectId == projectId
                               && pl.UserId == _currentUser.UserId
                               && pl.LeadRoleId == requiredLeadRole.Id);

            if (!isLead)
            {
                throw new UnauthorizedAccessException($"Only the '{requiredLeadRole.Name}' can manage assignments for this stage.");
            }
        }

        private async Task<FeaturePermissionOutput> GetUserPermissionsForFeature(int projectId)
        {
            var isPM = await _db.ProjectResources
                .AnyAsync(r => r.ProjectId == projectId && r.UserId == _currentUser.UserId && r.RoleId == RoleLookup.ProjectManager.Id);

            var leadStages = await _db.ProjectLeads
                .Where(pl => pl.ProjectId == projectId && pl.UserId == _currentUser.UserId)
                .Select(pl => pl.LeadRole.StageId)
                .ToListAsync();

            return new FeaturePermissionOutput
            {
                LeadStageIds = leadStages,
                IsPM = isPM
            };
        }

    }
}
