using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleaseStages.Commands.UpdateReleaseStage;

public record UpdateReleaseStageCommand(int ProjectId, int PlanId, int ReleaseId, int StageId, UpdateReleaseStageInput Input) : IRequest<ReleaseStageOutput>;