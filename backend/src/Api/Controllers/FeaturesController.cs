using Application.Features.Commands.AssignFeatureToRelease;
using Application.Features.Commands.AssignUserToStage;
using Application.Features.Commands.CreateFeature;
using Application.Features.Commands.DeleteFeature;
using Application.Features.Commands.MoveToNextStage;
using Application.Features.Commands.RejectAndMoveToPreviousStage;
using Application.Features.Commands.SetEstimation;
using Application.Features.Commands.StartStage;
using Application.Features.Commands.UnassignUserFromStage;
using Application.Features.Commands.UpdateFeature;
using Application.Features.Queries.GetAllFeatures;
using Application.Features.Queries.GetAvailableResourcesForStage;
using Application.Features.Queries.GetFeatureById;
using Application.Features.Queries.GetFeatureLogs;
using Application.Features.Queries.SearchFeatures;
using Application.Shared.DTOs.Feature;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId}/features")]
    public class FeaturesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeaturesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAllFeatures")]
        public async Task<IActionResult> GetAllFeatures(int projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            try
            {
                var features = await _mediator.Send(new GetAllFeaturesQuery(projectId, page, pageSize), ct);
                return Ok(features);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("SearchFeatures")]
        public async Task<IActionResult> SearchFeatures(int projectId, [FromQuery] string? name, [FromQuery] int? priority, [FromQuery] int? statusId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            try
            {
                var features = await _mediator.Send(new SearchFeaturesQuery(projectId, name, priority, statusId, page, pageSize), ct);
                return Ok(features);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("GetFeatureById/{featureId}")]
        [ProducesResponseType(typeof(FeatureDetailOutput), 200)]
        public async Task<IActionResult> GetFeatureById(int projectId, int featureId, CancellationToken ct)
        {
            try
            {
                var feature = await _mediator.Send(new GetFeatureByIdQuery(projectId, featureId), ct);

                if (feature == null)
                    return NotFound($"Feature with ID {featureId} not found.");

                return Ok(feature);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("CreateNewFeature")]
        public async Task<IActionResult> CreateNewFeature(int projectId, CreateFeatureInput input, CancellationToken ct)
        {
            try
            {
                var feature = await _mediator.Send(new CreateFeatureCommand(projectId, input), ct);
                return CreatedAtAction(nameof(GetFeatureById), new { projectId, featureId = feature.Id }, feature);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("UpdateFeature/{featureId}")]
        public async Task<IActionResult> UpdateFeature(int projectId, int featureId, UpdateFeatureInput input, CancellationToken ct)
        {
            try
            {
                var feature = await _mediator.Send(new UpdateFeatureCommand(projectId, featureId, input), ct);
                return Ok(feature);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("AssignFeatureToRelease/{featureId}")]
        public async Task<IActionResult> AssignFeatureToRelease(int projectId, int featureId, AssignFeatureReleaseInput input, CancellationToken ct)
        {
            try
            {
                var feature = await _mediator.Send(new AssignFeatureToReleaseCommand(projectId, featureId, input.ReleaseId), ct);
                return Ok(feature);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("delete-feature/{featureId}")]
        public async Task<IActionResult> DeleteFeature(int projectId, int featureId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteFeatureCommand(projectId, featureId), ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("stages/{stageId}/get-available-resources-for-stage", Name = "GetAvailableResources")]
        [ProducesResponseType(typeof(IEnumerable<AvailableStageResourceDto>), 200)]
        public async Task<IActionResult> GetAvailableResources([FromRoute] int projectId, [FromRoute] int stageId, CancellationToken ct)
        {
            try
            {
                var resources = await _mediator.Send(new GetAvailableResourcesForStageQuery(projectId, stageId), ct);
                return Ok(resources);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{featureId}/stages/{stageId}/assign-user-to-feature", Name = "AssignUserToStageInFeature")]
        public async Task<IActionResult> AssignUserToStage(int projectId, int featureId, int stageId, [FromBody] AssignUserDto input, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new AssignUserToStageCommand(projectId, featureId, stageId, input.AssignedUserId), ct);
                return Ok(new { message = "Resource assigned successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{featureId}/stages/{stageId}/unassign-user-from-feature", Name = "UnassignUserFromFeature")]
        public async Task<IActionResult> UnassignUserFromStage(int projectId, int featureId, int stageId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new UnassignUserFromStageCommand(projectId, featureId, stageId), ct);
                return Ok(new { message = "Resource unassigned successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{featureId}/stages/{stageId}/move-to-next-stage", Name = "MoveToNextStageInFeature")]
        public async Task<IActionResult> MoveToNextStage(int projectId, int featureId, int stageId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new MoveToNextStageCommand(projectId, featureId, stageId), ct);
                return Ok(new { message = "Stage completed and feature moved forward successfully." });
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{featureId}/stages/{stageId}/RejectFeatureStage", Name = "RejectFeatureStage")]
        public async Task<IActionResult> RejectStage(int projectId, int featureId, int stageId, [FromBody] RejectionInput input, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new RejectAndMoveToPreviousStageCommand(projectId, featureId, stageId, input.Comment), ct);
                return Ok(new { message = "Feature has been rejected and rolled back to Development successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{featureId}/GetFeatureLogs", Name = "GetFeatureLogs")]
        [ProducesResponseType(typeof(IEnumerable<FeatureStageLogOutput>), 200)]
        public async Task<IActionResult> GetFeatureLogs(int projectId, int featureId, CancellationToken ct)
        {
            try
            {
                var logs = await _mediator.Send(new GetFeatureLogsQuery(projectId, featureId), ct);
                return Ok(logs);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{featureId}/stages/{stageId}/start", Name = "StartStage")]
        public async Task<IActionResult> StartStage(int projectId, int featureId, int stageId)
        {
            await _mediator.Send(new StartStageCommand(featureId, stageId));
            return Ok();
        }

        [HttpPost("{featureId}/stages/{stageId}/estimation", Name = "SetEstimation")]
        public async Task<IActionResult> SetEstimation(int projectId, int featureId, int stageId, [FromBody] SetEstimationInput input)
        {
            await _mediator.Send(new SetEstimationCommand(featureId, stageId, input));
            return Ok();
        }
    }
}
