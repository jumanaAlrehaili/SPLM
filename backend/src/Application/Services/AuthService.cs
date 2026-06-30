using Application.Shared.Interfaces;
using Application.Services.Validators;
using Application.Shared.Dtos.Auth;
using Application.Shared.Dtos.Role;
using Application.Shared.Dtos.Users;
using Application.Shared.DTOs.Auth;
using Domain.IdentityEntities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IValidator<RegisterInputDto> _registerValidator;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;


    public AuthService(
        IAppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IValidator<RegisterInputDto> registerValidator,
        IConfiguration configuration,
        ITokenService tokenService,
        IEmailService emailService,
        IValidator<ResetPasswordDto> resetPasswordValidator)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _registerValidator = registerValidator;
        _configuration = configuration;
        _tokenService = tokenService;
        _emailService = emailService;
        _resetPasswordValidator = resetPasswordValidator;
    }

    public async Task<RegisterOutputDto> RegisterAsync(RegisterInputDto request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return new RegisterOutputDto(false, "Validation failed.", errors);
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegisterOutputDto(false, "Registration failed.",
                new[] { "A user with this email already exists." });
        }

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role == null)
        {
            return new RegisterOutputDto(false, "Registration failed.",
                new[] { $"Role with ID '{request.RoleId}' does not exist." });
        }

        var user = new ApplicationUser
        {
            FullName = request.Name,
            Email = request.Email,
            UserName = request.Email,
            //UserRoles = [new IdentityUserRole<int> { RoleId = request.RoleId }] 
        };

        await using var transaction = await _dbContext.BeginTransactionAsync();

        var createResult = await _userManager.CreateAsync(user, request.Password);
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

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        return await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(r.Id, r.Name!))
            .ToListAsync();
    }

    public async Task<LoginOutputDto> LoginAsync(LoginInputDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return new LoginOutputDto { Success = false };

        //Validate password and check for account lockout status, lockoutOnFailure: true increments the access failed count for the user
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);

        var response = new LoginOutputDto
        {
            Success = result.Succeeded,
            IsLockedOut = result.IsLockedOut
        };

        if (result.Succeeded)
        {
            // Roles table > Check UserRoles table to find which Role belongs to the UserId > return Role object (ID and Name).
            var userRole = await _roleManager.Roles
               .FirstOrDefaultAsync(r => r.UserRoles.Any(ur => ur.UserId == user.Id)); 

            if (userRole != null)
            {
                response.User = new UserDto
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user, userRole.Id, userRole.Name)
                };
            }
        }

        return response;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return true;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // link to angular                                            
        var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(token)}&email={user.Email}"; // Encodes special characters in the token to prevent URL parsing issues.

        var message = $@"<h2>Reset Your Password</h2>
                     <p>Please click the link below to reset your password:</p>
                     <a href='{resetLink}'>Reset Password</a>";

        await _emailService.SendEmailAsync(user.Email, "Reset Password Request", message);
        return true;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var validationResult = await _resetPasswordValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new IdentityError { Code = "ValidationError", Description = e.ErrorMessage });
            return IdentityResult.Failed(errors.ToArray());
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "User not found." });

        return await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
    }
}
