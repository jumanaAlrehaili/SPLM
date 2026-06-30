using Application.Shared.DTOs.ReleasePlan;
using MediatR;
using System.Collections.Generic;

namespace Application.ReleaseStages.Queries.GetReleaseStages;

public record GetReleaseStagesQuery(int ProjectId, int PlanId, int ReleaseId) : IRequest<IEnumerable<ReleaseStageOutput>>;