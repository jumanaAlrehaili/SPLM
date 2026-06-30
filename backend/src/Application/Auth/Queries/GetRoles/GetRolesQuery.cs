using Application.Shared.Dtos;
using Application.Shared.Dtos.Role;
using MediatR;
using System.Collections.Generic;

namespace Application.Auth.Queries.GetRoles;

public record GetRolesQuery : IRequest<IEnumerable<RoleDto>>;