using Application.Shared.Dtos.Role;
using Domain.IdentityEntities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<RoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetRolesQueryHandler(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        return await _roleManager.Roles
            .AsNoTracking() 
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(r.Id, r.Name!))
            .ToListAsync(ct);
    }
}