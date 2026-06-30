using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Commands.SetProjectLead;

public record SetProjectLeadCommand(int ProjectId, SetProjectLeadInput Input) : IRequest;
