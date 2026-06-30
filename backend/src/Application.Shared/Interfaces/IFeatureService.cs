using Application.Shared.DTOs.Feature;

namespace Application.Shared.Interfaces
{
    public interface IFeatureService
    {
        Task<IEnumerable<FeatureOutput>> GetAllFeaturesAsync(int projectId);
        Task<IEnumerable<FeatureOutput>> SearchFeaturesAsync(int projectId, string? name, int? priority, int? statusId);
        Task<FeatureDetailOutput?> GetFeatureByIdAsync(int projectId, int featureId);
        Task<FeatureDetailOutput> CreateFeatureAsync(int projectId, CreateFeatureInput input);
        Task<FeatureDetailOutput> UpdateFeatureAsync(int projectId, int featureId, UpdateFeatureInput input);
        Task DeleteFeatureAsync(int projectId, int featureId);
        Task AssignUserToStageAsync(int projectId, int featureId, int stageId, int assignedUserId);
        Task UnassignUserFromStageAsync(int projectId, int featureId, int stageId);
        Task<IEnumerable<AvailableStageResourceDto>> GetAvailableResourcesForStageAsync(int projectId, int stageId);
        Task MoveToNextStageInFeatureAsync(int projectId, int featureId, int currentStageId);
        Task RejectAndMoveToPreviousStageAsync(int projectId, int featureId, int currentStageId, string rejectionComment);
        Task<IEnumerable<FeatureStageLogOutput>> GetFeatureLogsAsync(int projectId, int featureId);
        Task<FeatureDetailOutput> AssignFeatureToReleaseAsync(int projectId, int featureId, int? releaseId);
    }
}
