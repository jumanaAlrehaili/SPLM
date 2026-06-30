using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleasePlans.Commands.UpdateReleasePlan;

public record UpdateReleasePlanCommand(int ProjectId, int PlanId, UpdateReleasePlanInput Input) : IRequest<ReleasePlanOutput>;