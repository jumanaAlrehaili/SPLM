using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleasePlans.Commands.CreateRelease;

public record CreateReleaseCommand(int ProjectId, int PlanId, CreateReleaseInput Input) : IRequest<ReleaseDetailOutput>;