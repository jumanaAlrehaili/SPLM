using Application.Lookups.Queries;
using Application.Shared.DTOs.Lookup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/lookups")]
    public class LookupsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LookupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("stages")]
        public async Task<ActionResult<IEnumerable<StageLookupDto>>> GetStages()
            => Ok(await _mediator.Send(new GetStagesQuery()));
        
        [Authorize]
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<StatusLookupDto>>> GetStatuses()
            => Ok(await _mediator.Send(new GetStatusesQuery()));

        [Authorize]
        [HttpGet("release-stage-statuses")]
        public async Task<ActionResult<IEnumerable<StatusLookupDto>>> GetReleaseStageStatuses()
            => Ok(await _mediator.Send(new GetReleaseStageStatusesQuery()));

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleLookupDto>>> GetRoles()
            => Ok(await _mediator.Send(new GetRolesQuery()));

        [Authorize]
        [HttpGet("feature-priorities")]
        public async Task<ActionResult<IEnumerable<FeaturePriorityDto>>> GetFeaturePriorities()
            => Ok(await _mediator.Send(new GetFeaturePrioritiesQuery()));

        [Authorize]
        [HttpGet("request-statuses")]
        public async Task<ActionResult<IEnumerable<RequestStatusDto>>> GetRequestStatuses()
            => Ok(await _mediator.Send(new GetRequestStatusesQuery()));

        [Authorize]
        [HttpGet("duration-units")]
        public async Task<ActionResult<IEnumerable<DurationUnitDto>>> GetDurationUnits()
            => Ok(await _mediator.Send(new GetDurationUnitsQuery()));

        [Authorize]
        [HttpGet("project-membership-statuses")]
        public async Task<ActionResult<IEnumerable<ProjectMembershipStatusDto>>> GetProjectMembershipStatuses()
            => Ok(await _mediator.Send(new GetProjectMembershipStatusesQuery()));

        [Authorize]
        [HttpGet("release-stage-change-types")]
        public async Task<ActionResult<IEnumerable<ReleaseStageChangeTypeDto>>> GetReleaseStageChangeTypes()
            => Ok(await _mediator.Send(new GetReleaseStageChangeTypesQuery()));

        [Authorize]
        [HttpGet("estimation-units")] 
        public async Task<ActionResult<IEnumerable<EstimationUnitDto>>> GetEstimationUnits()
           => Ok(await _mediator.Send(new GetEstimationUnitsQuery()));
    }
}