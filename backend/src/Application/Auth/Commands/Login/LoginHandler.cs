using Application.Shared.Dtos.Auth;
using Application.Shared.Dtos.Users;
using Application.Shared.Interfaces;
using Domain.IdentityEntities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginOutputDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<LoginOutputDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Input.Email);
        if (user == null) return new LoginOutputDto { Success = false };

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Input.Password, lockoutOnFailure: true);

        var response = new LoginOutputDto
        {
            Success = result.Succeeded,
            IsLockedOut = result.IsLockedOut
        };

        if (result.Succeeded)
        {
            var userRole = await _roleManager.Roles
               .AsNoTracking()
               .FirstOrDefaultAsync(r => r.UserRoles.Any(ur => ur.UserId == user.Id), ct);

            if (userRole != null)
            {
                response.User = new UserDto
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user, userRole.Id, userRole.Name!)
                };
            }
        }

        return response;
    }
}