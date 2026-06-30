using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleasePlans.Commands.CreateReleasePlan;

public record CreateReleasePlanCommand(int ProjectId, CreateReleasePlanInput Input) : IRequest<ReleasePlanOutput>;