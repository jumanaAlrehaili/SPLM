using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleasePlans.Commands.UpdateRelease;

public record UpdateReleaseCommand(int ProjectId, int PlanId, int ReleaseId, UpdateReleaseInput Input) : IRequest<ReleaseDetailOutput>;