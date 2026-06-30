using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.ReleasePlans.Commands.CreateReleasePlan;
using Application.ReleasePlans.Commands.UpdateReleasePlan;
using Application.ReleasePlans.Commands.DeleteReleasePlan;
using Application.ReleasePlans.Commands.CreateRelease;
using Application.ReleasePlans.Commands.UpdateRelease;
using Application.ReleasePlans.Commands.DeleteRelease;
using Application.ReleasePlans.Queries.GetReleasePlans;
using Application.ReleasePlans.Queries.GetReleasePlanById;
using Application.ReleasePlans.Queries.GetReleases;
using Application.ReleasePlans.Queries.GetReleaseById;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId}/release-plans")]
    public class ReleasePlansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReleasePlansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ── Release Plans ────────────────────────────────────────

        [HttpGet("GetReleasePlans", Name = "GetReleasePlans")]
        public async Task<IActionResult> GetReleasePlans([FromRoute] int projectId, CancellationToken ct)
        {
            try
            {
                var plans = await _mediator.Send(new GetReleasePlansQuery(projectId), ct);
                return Ok(plans);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("GetReleasePlanById/{planId}", Name = "GetReleasePlanById")]
        public async Task<IActionResult> GetReleasePlanById([FromRoute] int projectId, [FromRoute] int planId, CancellationToken ct)
        {
            try
            {
                var plan = await _mediator.Send(new GetReleasePlanByIdQuery(projectId, planId), ct);
                if (plan == null) return NotFound($"Release plan with ID {planId} not found.");
                return Ok(plan);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("CreateReleasePlan", Name = "CreateReleasePlan")]
        public async Task<IActionResult> CreateReleasePlan([FromRoute] int projectId, CreateReleasePlanInput input, CancellationToken ct)
        {
            try
            {
                var plan = await _mediator.Send(new CreateReleasePlanCommand(projectId, input), ct);
                return CreatedAtAction(nameof(GetReleasePlanById), new { projectId, planId = plan.Id }, plan);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("UpdateReleasePlan/{planId}", Name = "UpdateReleasePlan")]
        public async Task<IActionResult> UpdateReleasePlan([FromRoute] int projectId, [FromRoute] int planId, UpdateReleasePlanInput input, CancellationToken ct)
        {
            try
            {
                var plan = await _mediator.Send(new UpdateReleasePlanCommand(projectId, planId, input), ct);
                return Ok(plan);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("DeleteReleasePlan/{planId}", Name = "DeleteReleasePlan")]
        public async Task<IActionResult> DeleteReleasePlan([FromRoute] int projectId, [FromRoute] int planId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteReleasePlanCommand(projectId, planId), ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // ── Releases ─────────────────────────────────────────────

        [HttpGet("{planId}/GetAllReleases", Name = "GetAllReleases")]
        public async Task<IActionResult> GetAllReleases([FromRoute] int projectId, [FromRoute] int planId, CancellationToken ct)
        {
            try
            {
                var releases = await _mediator.Send(new GetReleasesQuery(projectId, planId), ct);
                return Ok(releases);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{planId}/GetReleaseById/{releaseId}", Name = "GetReleaseById")]
        public async Task<IActionResult> GetReleaseById([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, CancellationToken ct)
        {
            try
            {
                var release = await _mediator.Send(new GetReleaseByIdQuery(projectId, planId, releaseId), ct);
                if (release == null) return NotFound($"Release with ID {releaseId} not found.");
                return Ok(release);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{planId}/CreateRelease", Name = "CreateRelease")]
        public async Task<IActionResult> CreateRelease([FromRoute] int projectId, [FromRoute] int planId, CreateReleaseInput input, CancellationToken ct)
        {
            try
            {
                var release = await _mediator.Send(new CreateReleaseCommand(projectId, planId, input), ct);
                return CreatedAtAction(nameof(GetReleaseById), new { projectId, planId, releaseId = release.Id }, release);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{planId}/UpdateRelease/{releaseId}", Name = "UpdateRelease")]
        public async Task<IActionResult> UpdateRelease([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, UpdateReleaseInput input, CancellationToken ct)
        {
            try
            {
                var release = await _mediator.Send(new UpdateReleaseCommand(projectId, planId, releaseId, input), ct);
                return Ok(release);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{planId}/DeleteRelease/{releaseId}", Name = "DeleteRelease")]
        public async Task<IActionResult> DeleteRelease([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteReleaseCommand(projectId, planId, releaseId), ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}