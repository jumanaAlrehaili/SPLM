using Application.Shared.Dtos.Auth;
using Application.Shared.Interfaces;
using Domain.IdentityEntities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterOutputDto>
{
    private readonly IAppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IValidator<RegisterInputDto> _registerValidator;

    public RegisterHandler(
        IAppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IValidator<RegisterInputDto> registerValidator)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _registerValidator = registerValidator;
    }

    public async Task<RegisterOutputDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        var validationResult = await _registerValidator.ValidateAsync(request.Input, ct);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return new RegisterOutputDto(false, "Validation failed.", errors);
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Input.Email);
        if (existingUser != null)
        {
            return new RegisterOutputDto(false, "Registration failed.", new[] { "A user with this email already exists." });
        }

        var role = await _roleManager.FindByIdAsync(request.Input.RoleId.ToString());
        if (role == null)
        {
            return new RegisterOutputDto(false, "Registration failed.", new[] { $"Role with ID '{request.Input.RoleId}' does not exist." });
        }

        var user = new ApplicationUser
        {
            FullName = request.Input.Name,
            Email = request.Input.Email,
            UserName = request.Input.Email
        };

        await using var transaction = await _dbContext.BeginTransactionAsync();

        var createResult = await _userManager.CreateAsync(user, request.Input.Password);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            var errors = createResult.Errors.Select(e => e.Description).ToArray();
            return new RegisterOutputDto(false, "Registration failed.", errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role.Name!);
        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync();
            var errors = roleResult.Errors.Select(e => e.Description).ToArray();
            return new RegisterOutputDto(false, "Registration failed.", errors);
        }

        await transaction.CommitAsync();

        return new RegisterOutputDto(true, "User registered successfully.", Array.Empty<string>(), new[] { role.Name! });
    }
}