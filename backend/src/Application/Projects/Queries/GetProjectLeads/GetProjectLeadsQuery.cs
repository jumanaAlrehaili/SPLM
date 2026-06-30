using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetProjectLeads;

public record GetProjectLeadsQuery(int ProjectId) : IRequest<IEnumerable<ProjectLeadOutput>>;
