using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.ReleaseStages.Queries.GetReleaseStages;
using Application.ReleaseStages.Commands.CreateReleaseStage;
using Application.ReleaseStages.Commands.UpdateReleaseStage;
using Application.ReleaseStages.Commands.MoveReleaseStage;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId}/release-plans/{planId}/releases/{releaseId}/stages")]
    public class ReleaseStagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReleaseStagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllReleaseStages", Name = "GetAllReleaseStages")]
        public async Task<IActionResult> GetAllReleaseStages([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, CancellationToken ct)
        {
            try
            {
                var stages = await _mediator.Send(new GetReleaseStagesQuery(projectId, planId, releaseId), ct);
                return Ok(stages);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("CreateReleaseStage", Name = "CreateReleaseStage")]
        public async Task<IActionResult> CreateReleaseStage([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, [FromBody] CreateReleaseStageInput input, CancellationToken ct)
        {
            try
            {
                var stage = await _mediator.Send(new CreateReleaseStageCommand(projectId, planId, releaseId, input), ct);
                return CreatedAtRoute("GetAllReleaseStages", new { projectId, planId, releaseId }, stage);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("UpdateReleaseStage/{stageId}", Name = "UpdateReleaseStage")]
        public async Task<IActionResult> UpdateReleaseStage([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, [FromRoute] int stageId, [FromBody] UpdateReleaseStageInput input, CancellationToken ct)
        {
            try
            {
                var stage = await _mediator.Send(new UpdateReleaseStageCommand(projectId, planId, releaseId, stageId, input), ct);
                return Ok(stage);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("MoveReleaseStage/{stageId}", Name = "MoveReleaseStage")]
        public async Task<IActionResult> MoveReleaseStage([FromRoute] int projectId, [FromRoute] int planId, [FromRoute] int releaseId, [FromRoute] int stageId, [FromQuery] string? notes, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new MoveReleaseStageCommand(projectId, planId, releaseId, stageId, notes), ct);
                return Ok(new { message = "Stage advanced successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}