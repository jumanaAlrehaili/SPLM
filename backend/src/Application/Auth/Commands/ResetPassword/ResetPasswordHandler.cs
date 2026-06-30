using Application.Shared.Dtos.Auth;
using Application.Shared.DTOs.Auth;
using Domain.IdentityEntities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, IdentityResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;

    public ResetPasswordHandler(UserManager<ApplicationUser> userManager, IValidator<ResetPasswordDto> resetPasswordValidator)
    {
        _userManager = userManager;
        _resetPasswordValidator = resetPasswordValidator;
    }

    public async Task<IdentityResult> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var validationResult = await _resetPasswordValidator.ValidateAsync(request.Dto, ct);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new IdentityError { Code = "ValidationError", Description = e.ErrorMessage });
            return IdentityResult.Failed(errors.ToArray());
        }

        var user = await _userManager.FindByEmailAsync(request.Dto.Email);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "User not found." });

        return await _userManager.ResetPasswordAsync(user, request.Dto.Token, request.Dto.NewPassword);
    }
}