using Application.Shared.DTOs.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.Interfaces
{
    public interface IProjectService
    {
        // Create project and add PM as first resource
        Task<int> CreateProjectAsync(CreateProjectInput input);

        // Get projects for "All Projects" tab
        Task<IEnumerable<ProjectOutput>> GetAllProjectsAsync();

        Task<IEnumerable<ProjectOutput>> SearchProjectsAsync(string? name, DateTime? createdFrom, DateTime? createdTo);

        // Get projects for "My Projects" tab
        Task<IEnumerable<ProjectOutput>> GetMyProjectsAsync();

        // Get single project details
        Task<ProjectOutput?> GetProjectByIdAsync(int id);

        Task<int> SubmitJoinRequestAsync(SubmitJoinRequestInput input);

        Task<IEnumerable<JoinRequestOutput>> GetPendingRequestsForManagerAsync();
        Task<IEnumerable<JoinRequestOutput>> GetMyJoinRequestsAsync();

        Task ApproveRequestAsync(int requestId);

        Task RejectRequestAsync(int requestId);

        Task DeleteProjectAsync(int id);
        Task<IEnumerable<ProjectLeadOutput>> GetProjectLeadsAsync(int projectId);
        Task SetProjectLeadAsync(int projectId, SetProjectLeadInput input);
    }
}
