using Application.Shared.DTOs.ReleasePlan;
using MediatR;
using System.Collections.Generic;

namespace Application.ReleasePlans.Queries.GetReleasePlans;

public record GetReleasePlansQuery(int ProjectId) : IRequest<IEnumerable<ReleasePlanOutput>>;