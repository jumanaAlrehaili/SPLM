using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Entities.Projects;
using Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IAppDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public ProjectService(IAppDbContext dbContext, ICurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<int> CreateProjectAsync(CreateProjectInput input)
        {
            //var pmRole = await _dbContext.Roles
            //    .FirstOrDefaultAsync(r => r.Id == RoleLookup.ProjectManager.Id);

            //if (pmRole == null)
            //{
            //    throw new Exception("The required role was not found in the database.");
            //}

            // Map InputDTO to Project Entity
            if (input.DurationUnit.HasValue && !DurationUnitsLookup.All.Any(d => d.Id == input.DurationUnit.Value))
                throw new InvalidOperationException("Invalid duration unit.");

            var project = new Project
            {
                Name = input.Name,
                Description = input.Description,
                Budget = input.Budget,
                Duration = input.Duration,
                DurationUnitId = input.DurationUnit,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId
            };
            // Add #1
            project.ProjectResources.Add(new ProjectResource
            {
                UserId = _currentUser.UserId, //from token
                RoleId = RoleLookup.ProjectManager.Id, // Assign the creator as "Project Manager" in resources table.
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId //from token
            });

            // Add #2
            _dbContext.Projects.Add(project); 
            await _dbContext.SaveChangesAsync(); //one transaction

            return project.Id;
        }

        // Retrieves all projects with the relation status for the current user.
        public async Task<IEnumerable<ProjectOutput>> GetAllProjectsAsync()
        {
            var currentUserId = _currentUser.UserId; //from token

            var projects = await _dbContext.Projects
                .AsNoTracking()
                .Select(p => new ProjectOutput //without include.
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Budget = p.Budget,
                    Duration = p.Duration,
                    DurationUnit = p.Duration != null ? p.DurationUnit.Name : null,
                    CreatedAt = p.CreatedAt,
                    CreatorName = p.CreatedByUser.FullName,
                    ResourcesCount = p.ProjectResources.Count(),

                    // if "owner": show "manage project" button and dont show "join" button
                    // if "None": show "join" button.
                    MembershipStatusId =
                        p.ProjectResources.Any(r => r.UserId == currentUserId && r.RoleId == RoleLookup.ProjectManager.Id) ? ProjectMembershipStatusLookup.Owner.Id
                      : p.ProjectResources.Any(r => r.UserId == currentUserId) ? ProjectMembershipStatusLookup.Member.Id
                      : p.ProjectJoinRequests.Any(j => j.UserId == currentUserId && j.StatusId == RequestStatusesLookup.Pending.Id) ? ProjectMembershipStatusLookup.Pending.Id
                      : ProjectMembershipStatusLookup.None.Id,
                })
                .ToListAsync();
            return projects;
        }

        public async Task<IEnumerable<ProjectOutput>> SearchProjectsAsync(string? name, DateTime? createdFrom, DateTime? createdTo)
        {
            var currentUserId = _currentUser.UserId;

            return await _dbContext.Projects
                .AsNoTracking()
                .Where(p => string.IsNullOrEmpty(name) || p.Name.Contains(name))
                .Where(p => createdFrom == null || p.CreatedAt >= createdFrom)
                .Where(p => createdTo == null || p.CreatedAt <= createdTo)
                .Select(p => new ProjectOutput
                {
                    Id              = p.Id,
                    Name            = p.Name,
                    Description     = p.Description,
                    Budget          = p.Budget,
                    Duration        = p.Duration,
                    DurationUnit = p.Duration != null ? p.DurationUnit.Name : null,
                    CreatedAt       = p.CreatedAt,
                    CreatorName     = p.CreatedByUser.FullName,
                    ResourcesCount  = p.ProjectResources.Count(),
                    MembershipStatusId =
                        p.ProjectResources.Any(r => r.UserId == currentUserId && r.RoleId == RoleLookup.ProjectManager.Id) ? ProjectMembershipStatusLookup.Owner.Id
                      : p.ProjectResources.Any(r => r.UserId == currentUserId) ? ProjectMembershipStatusLookup.Member.Id
                      : p.ProjectJoinRequests.Any(j => j.UserId == currentUserId && j.StatusId == RequestStatusesLookup.Pending.Id) ? ProjectMembershipStatusLookup.Pending.Id
                      : ProjectMembershipStatusLookup.None.Id,
                })
                .ToListAsync();
        }

        // only projects where the current user is either the Owner(creator) or a Member.
        public async Task<IEnumerable<ProjectOutput>> GetMyProjectsAsync()
        {
            var currentUserId = _currentUser.UserId; 

            var projects = await _dbContext.Projects //where - select
                .AsNoTracking()
                .Where(p => p.ProjectResources.Any(r => r.UserId == currentUserId)) //PM or any role
                .Select(p => new ProjectOutput
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Budget = p.Budget,
                    Duration = p.Duration,
                    DurationUnit = p.Duration != null ? p.DurationUnit.Name : null,
                    CreatedAt = p.CreatedAt,
                    CreatorName = p.CreatedByUser.FullName,
                    ResourcesCount = p.ProjectResources.Count(),

                    MembershipStatusId =
                        p.ProjectResources.Any(r => r.UserId == currentUserId && r.RoleId == RoleLookup.ProjectManager.Id) ? ProjectMembershipStatusLookup.Owner.Id
                      : p.ProjectResources.Any(r => r.UserId == currentUserId) ? ProjectMembershipStatusLookup.Member.Id
                      : p.ProjectJoinRequests.Any(j => j.UserId == currentUserId && j.StatusId == RequestStatusesLookup.Pending.Id) ? ProjectMembershipStatusLookup.Pending.Id
                      : ProjectMembershipStatusLookup.None.Id,
                })
                .ToListAsync();

            return projects;
        }

        public async Task<ProjectOutput?> GetProjectByIdAsync(int id)
        {
            var currentUserId = _currentUser.UserId;

            //is lead?
            var projectLeadUserIds = await _dbContext.ProjectLeads
                .AsNoTracking()
                .Where(pl => pl.ProjectId == id)
                .Select(pl => pl.UserId)
                .ToListAsync();

            var project = await _dbContext.Projects
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProjectOutput
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Budget = p.Budget,
                    Duration = p.Duration,
                    DurationUnit = p.Duration != null ? p.DurationUnit.Name : null,
                    CreatedAt = p.CreatedAt,
                    CreatorName = p.CreatedByUser.FullName,

                    MembershipStatusId =
                        p.ProjectResources.Any(r => r.UserId == currentUserId && r.RoleId == RoleLookup.ProjectManager.Id) ? ProjectMembershipStatusLookup.Owner.Id
                      : p.ProjectResources.Any(r => r.UserId == currentUserId) ? ProjectMembershipStatusLookup.Member.Id
                      : p.ProjectJoinRequests.Any(j => j.UserId == currentUserId && j.StatusId == RequestStatusesLookup.Pending.Id) ? ProjectMembershipStatusLookup.Pending.Id
                      : ProjectMembershipStatusLookup.None.Id,

                    Resources = p.ProjectResources.Select(r => new ProjectResourceDto
                    {
                      UserId = r.UserId,
                      UserName = r.User.FullName,
                      RoleId = r.RoleId,
                      RoleName = r.Role.Name,
                      IsLeader = projectLeadUserIds.Contains(r.UserId)
                    }).ToList()
                })
               .FirstOrDefaultAsync();

            if (project != null)
            {
                project.GroupedResources = GroupedResources(project.Resources);
            }
            return project;
        }

        // ---------- Project Join Request ----------

        public async Task<int> SubmitJoinRequestAsync(SubmitJoinRequestInput input)
        {
            var currentUserId = _currentUser.UserId;
            var roleIdFromToken = _currentUser.RoleId;

            var role = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roleIdFromToken) ?? throw new Exception("Role not found");


            var isAlreadyMember = await _dbContext.ProjectResources
                .AnyAsync(r => r.ProjectId == input.ProjectId && r.UserId == currentUserId);

            if (isAlreadyMember)
                throw new Exception("You are already part of this project.");

            var existingRequest = await _dbContext.ProjectJoinRequests
                .AnyAsync(r => r.ProjectId == input.ProjectId && r.UserId == currentUserId && r.StatusId == RequestStatusesLookup.Pending.Id);

            if (existingRequest)
                throw new Exception("You already have a pending request for this project.");

            var request = new ProjectJoinRequest
            {
                ProjectId = input.ProjectId,
                UserId = currentUserId,
                RoleId = role.Id,
                JoinReason = input.JoinReason,
                StatusId = RequestStatusesLookup.Pending.Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ProjectJoinRequests.Add(request);
            await _dbContext.SaveChangesAsync();

            return request.Id;
        }

        public async Task<IEnumerable<JoinRequestOutput>> GetPendingRequestsForManagerAsync()
        {
            var currentUserId = _currentUser.UserId;

            return await _dbContext.ProjectJoinRequests 
                .AsNoTracking()
                .Where(r => r.Project.ProjectResources.Any(res => res.UserId == currentUserId && res.RoleId == RoleLookup.ProjectManager.Id) 
                            && r.StatusId == RequestStatusesLookup.Pending.Id) 
                .Select(r => new JoinRequestOutput
                {
                    Id = r.Id,
                    ProjectName = r.Project.Name,
                    UserName = r.User.FullName,
                    JoinReason = r.JoinReason,
                    RequestedAt = r.CreatedAt,
                    RoleName = r.Role.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<JoinRequestOutput>> GetMyJoinRequestsAsync()
        {
            var currentUserId = _currentUser.UserId;

            return await _dbContext.ProjectJoinRequests
                .AsNoTracking()
                .Where(r => r.UserId == currentUserId)
                .Select(r => new JoinRequestOutput
                {
                    Id          = r.Id,
                    ProjectName = r.Project.Name,
                    UserName    = r.User.FullName,
                    RoleName    = r.Role.Name,
                    JoinReason  = r.JoinReason ?? string.Empty,
                    RequestedAt = r.CreatedAt,
                    Status      = r.Status.Name
                })
                .ToListAsync();
        }

        // Approve a request and automatically add the user as a Project Resource
        public async Task ApproveRequestAsync(int requestId)
        {
            var currentUserId = _currentUser.UserId;

            var request = await _dbContext.ProjectJoinRequests
                .FirstOrDefaultAsync(r => r.Id == requestId); 

            if (request == null || request.StatusId != RequestStatusesLookup.Pending.Id)
                throw new Exception("Request not found or has already been processed.");

            // Check if the current user is PM for the requested Project
            var isManager = await _dbContext.ProjectResources
                .AnyAsync(res => res.ProjectId == request.ProjectId &&
                                 res.UserId == currentUserId &&
                                 res.RoleId == RoleLookup.ProjectManager.Id);

            if (!isManager)
                throw new UnauthorizedAccessException("You do not have permission to approve requests for this project.");

            var alreadyAdded = await _dbContext.ProjectResources
                .AnyAsync(r => r.ProjectId == request.ProjectId &&
                               r.UserId == request.UserId);

            if (alreadyAdded)
                throw new Exception("The user is already added to the project. ");

            // Update request status to Approved
            request.StatusId = RequestStatusesLookup.Approved.Id;

            var newResource = new ProjectResource
            {
                ProjectId = request.ProjectId,
                UserId = request.UserId,
                RoleId = request.RoleId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId
            };

            _dbContext.ProjectResources.Add(newResource);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RejectRequestAsync(int requestId)
        {   
            var currentUserId = _currentUser.UserId;

            var request = await _dbContext.ProjectJoinRequests
                .FirstOrDefaultAsync(r => r.Id == requestId); 

            if (request == null || request.StatusId != RequestStatusesLookup.Pending.Id)
                throw new Exception("Request not found or has already been processed.");

            var isManager = await _dbContext.ProjectResources
                .AnyAsync(res => res.ProjectId == request.ProjectId &&
                                 res.UserId == currentUserId &&
                                 res.RoleId == RoleLookup.ProjectManager.Id);

            if (!isManager)
                throw new UnauthorizedAccessException("You do not have permission to reject requests for this project.");


            // Update the status to Rejected
            request.StatusId = RequestStatusesLookup.Rejected.Id;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var currentUserId = _currentUser.UserId;

            var project = await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                throw new Exception("Project not found.");

            var isManager = await _dbContext.ProjectResources
                .AnyAsync(res => res.ProjectId == id &&
                         res.UserId == currentUserId &&
                         res.RoleId == RoleLookup.ProjectManager.Id);

            if (!isManager)
                throw new UnauthorizedAccessException("Only an active Project Manager can delete this project.");

            _dbContext.Projects.Remove(project); //with the related data in ProjectResources and ProjectJoinRequests(any status).
            await _dbContext.SaveChangesAsync();
        }

        // ---------- Project Leads ----------

        public async Task<IEnumerable<ProjectLeadOutput>> GetProjectLeadsAsync(int projectId)
        {         
            var isMember = await _dbContext.ProjectResources 
                .AnyAsync(r => r.ProjectId == projectId && r.UserId == _currentUser.UserId); 

            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this project.");

            var leads = await _dbContext.LeadRoles
                .AsNoTracking()
                .Select(lr => new ProjectLeadOutput
                {   //LeadRoles: get all 5 lead roles  
                    LeadRoleId = lr.Id,
                    LeadRoleName = lr.Name,
                    StageId = lr.StageId,

                    //ProjectLeads: get the assigned user for this role in the current project
                    UserId = lr.ProjectLeads
                        .Where(pl => pl.ProjectId == projectId)
                        .Select(pl => (int?)pl.UserId) // if not assigned yet, return null
                        .FirstOrDefault(),

                    UserName = lr.ProjectLeads
                        .Where(pl => pl.ProjectId == projectId)
                        .Select(pl => pl.User.FullName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return leads;
        }

        // Assign or update a specific lead role (Only by PM)
        public async Task SetProjectLeadAsync(int projectId, SetProjectLeadInput input)
        {
            var isManager = await _dbContext.ProjectResources
                .AnyAsync(res => res.ProjectId == projectId &&
                                 res.UserId == _currentUser.UserId &&
                                 res.RoleId == RoleLookup.ProjectManager.Id);

            if (!isManager)
                throw new UnauthorizedAccessException("Only the Project Manager can assign leaders.");

            var isResource = await _dbContext.ProjectResources
                .AnyAsync(pr => pr.ProjectId == projectId && pr.UserId == input.UserId);

            if (!isResource)
                throw new KeyNotFoundException("The selected user is not a resource in this project. Add them to the project first.");

            var roleExists = await _dbContext.LeadRoles.AnyAsync(r => r.Id == input.LeadRoleId);

            if (!roleExists)
                throw new KeyNotFoundException("Invalid lead role specified.");

            // Check if this specific role already has an assignment in the project
            var existingLead = await _dbContext.ProjectLeads
                .FirstOrDefaultAsync(pl => pl.ProjectId == projectId && pl.LeadRoleId == input.LeadRoleId);

            if (existingLead != null) //update
            {
                existingLead.UserId = input.UserId;
                existingLead.UpdatedAt = DateTime.UtcNow;
                existingLead.UpdatedByUserId = _currentUser.UserId;
            }
            else
            {
                var newLead = new ProjectLead //create 
                {
                    ProjectId = projectId,
                    LeadRoleId = input.LeadRoleId,
                    UserId = input.UserId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUser.UserId
                };
                _dbContext.ProjectLeads.Add(newLead);
            }

            await _dbContext.SaveChangesAsync();
        }

        //-------helpers-------
        private List<ProjectTeamsOutput> GroupedResources(IEnumerable<ProjectResourceDto> resources)
        {
            // Grouping by RoleId 
            return resources
                .GroupBy(r => r.RoleId)
                .OrderBy(roleGroup => roleGroup.Key) // RoleId (Key)
                .Select(roleGroup => new ProjectTeamsOutput
                {
                    //take the name from the first element of the group 
                    RoleName = roleGroup.First().RoleName,
                    Members = roleGroup
                    .OrderByDescending(m => m.IsLeader) //leader first
                    .Select(m => new ProjectResourceDto
                    {
                        UserId = m.UserId,
                        UserName = m.UserName,
                        IsLeader = m.IsLeader
                    }).ToList()
                })
                .ToList();
        }

        private int GetMembershipStatus(int? userRoleId, bool hasPendingJoinRequest)
        {
            if (userRoleId == null)
                return hasPendingJoinRequest
                    ? ProjectMembershipStatusLookup.Pending.Id
                    : ProjectMembershipStatusLookup.None.Id;

            return userRoleId == RoleLookup.ProjectManager.Id
                ? ProjectMembershipStatusLookup.Owner.Id
                : ProjectMembershipStatusLookup.Member.Id;
        }
    }
}
