using Application.Shared.DTOs.ReleasePlan;

namespace Application.Shared.Interfaces
{
    public interface IReleasePlanService
    {
        // Release Plans
        Task<IEnumerable<ReleasePlanOutput>> GetReleasePlansAsync(int projectId);
        Task<ReleasePlanDetailOutput?> GetReleasePlanByIdAsync(int projectId, int planId);
        Task<ReleasePlanOutput> CreateReleasePlanAsync(int projectId, CreateReleasePlanInput input);
        Task<ReleasePlanOutput> UpdateReleasePlanAsync(int projectId, int planId, UpdateReleasePlanInput input);
        Task DeleteReleasePlanAsync(int projectId, int planId);

        // Releases
        Task<IEnumerable<ReleaseOutput>> GetReleasesAsync(int projectId, int planId);
        Task<ReleaseDetailOutput?> GetReleaseByIdAsync(int projectId, int planId, int releaseId);
        Task<ReleaseDetailOutput> CreateReleaseAsync(int projectId, int planId, CreateReleaseInput input);
        Task<ReleaseDetailOutput> UpdateReleaseAsync(int projectId, int planId, int releaseId, UpdateReleaseInput input);
        Task DeleteReleaseAsync(int projectId, int planId, int releaseId);
    }
}
