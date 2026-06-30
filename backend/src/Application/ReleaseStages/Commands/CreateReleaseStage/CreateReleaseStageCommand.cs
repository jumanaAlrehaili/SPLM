using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleaseStages.Commands.CreateReleaseStage;

public record CreateReleaseStageCommand(int ProjectId, int PlanId, int ReleaseId, CreateReleaseStageInput Input) : IRequest<ReleaseStageOutput>;
