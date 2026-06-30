using Application.Projects.Commands.ApproveRequest;
using Application.Projects.Commands.CreateProject;
using Application.Projects.Commands.DeleteProject;
using Application.Projects.Commands.RejectRequest;
using Application.Projects.Commands.SetProjectLead;
using Application.Projects.Commands.SubmitJoinRequest;
using Application.Projects.Queries.GetAllProjects;
using Application.Projects.Queries.GetMyJoinRequests;
using Application.Projects.Queries.GetMyProjects;
using Application.Projects.Queries.GetPendingRequests;
using Application.Projects.Queries.GetProjectById;
using Application.Projects.Queries.GetProjectLeads;
using Application.Projects.Queries.GetResources;
using Application.Projects.Queries.SearchProjects;
using Application.Projects.Queries.SearchResources;
using Application.Shared.DTOs.Project;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateNewProject")]
        public async Task<IActionResult> CreateProject(CreateProjectInput input, CancellationToken ct)
        {
            try
            {
                var projectId = await _mediator.Send(new CreateProjectCommand(input), ct);
                return Ok(projectId);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("GetAllProjects")]
        public async Task<IActionResult> GetAllProjects([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var projects = await _mediator.Send(new GetAllProjectsQuery(page, pageSize), ct);
            return Ok(projects);
        }

        [HttpGet("SearchProjects")]
        public async Task<IActionResult> SearchProjects([FromQuery] string? name, [FromQuery] DateTime? createdFrom, [FromQuery] DateTime? createdTo, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var projects = await _mediator.Send(new SearchProjectsQuery(name, createdFrom, createdTo, page, pageSize), ct);
            return Ok(projects);
        }

        [HttpGet("GetMyProjects")]
        public async Task<IActionResult> GetMyProjects([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var projects = await _mediator.Send(new GetMyProjectsQuery(page, pageSize), ct);
            return Ok(projects);
        }

        [HttpGet("GetProjectById/{id}")]
        [ProducesResponseType(typeof(ProjectOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjectById([FromRoute] int id, CancellationToken ct)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(id), ct);

            if (project == null)
                return NotFound($"Project with ID {id} not found.");

            return Ok(project);
        }

        // ---------- Project Join Request ----------

        [HttpPost("SubmitNewJoinRequest")]
        public async Task<IActionResult> JoinProject(SubmitJoinRequestInput input, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new SubmitJoinRequestCommand(input), ct);
                return Ok(new { message = "Join request submitted successfully." });
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("get-pending-requests")]
        public async Task<IActionResult> GetPendingRequests(CancellationToken ct)
        {
            var requests = await _mediator.Send(new GetPendingRequestsQuery(), ct);
            return Ok(requests);
        }

        [HttpGet("GetMyJoinRequests")]
        public async Task<IActionResult> GetMyJoinRequests(CancellationToken ct)
        {
            var requests = await _mediator.Send(new GetMyJoinRequestsQuery(), ct);
            return Ok(requests);
        }

        [HttpPost("approve-request/{id}")]
        public async Task<IActionResult> ApproveRequest([FromRoute] int id, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new ApproveRequestCommand(id), ct);
                return Ok(new { message = "User added to the project successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("reject-request/{id}")]
        public async Task<IActionResult> RejectRequest([FromRoute] int id, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new RejectRequestCommand(id), ct);
                return Ok(new { message = "Request rejected." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("delete-project/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int id, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteProjectCommand(id), ct);
                return Ok(new { message = "Project and all related data deleted successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // ---------- Resources ----------

        [HttpGet("GetResources", Name = "GetResources")]
        public async Task<IActionResult> GetResources([FromQuery] int? projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var resources = await _mediator.Send(new GetResourcesQuery(projectId, page, pageSize), ct);
            return Ok(resources);
        }

        [HttpGet("SearchResources", Name = "SearchResources")]
        public async Task<IActionResult> SearchResources([FromQuery] string? searchTerm, [FromQuery] List<int>? projectId, [FromQuery] List<int>? roleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var resources = await _mediator.Send(new SearchResourcesQuery(searchTerm, projectId, roleId, page, pageSize), ct);
            return Ok(resources);
        }

        // ---------- Project Leads ----------

        [HttpGet("{projectId}/get-project-leads", Name = "GetProjectLeads")]
        public async Task<IActionResult> GetProjectLeads([FromRoute] int projectId, CancellationToken ct)
        {
            try
            {
                var leads = await _mediator.Send(new GetProjectLeadsQuery(projectId), ct);
                return Ok(leads);
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{projectId}/assign-project-leads", Name = "AssignProjectLeads")]
        public async Task<IActionResult> SetProjectLead([FromRoute] int projectId, [FromBody] SetProjectLeadInput input, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new SetProjectLeadCommand(projectId, input), ct);
                return Ok(new { message = "Project lead assigned successfully." });
            }
            catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
