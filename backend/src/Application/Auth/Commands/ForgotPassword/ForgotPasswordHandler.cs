using Application.Shared.Interfaces;
using Domain.IdentityEntities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService; 

    public ForgotPasswordHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return true; 

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(token)}&email={user.Email}";

        var message = $@"<h2>Reset Your Password</h2>
                     <p>Please click the link below to reset your password:</p>
                     <a href='{resetLink}'>Reset Password</a>";

        await _emailService.SendEmailAsync(user.Email!, "Reset Password Request", message);
        return true;
    }
}