using Application.Auth.Commands.ForgotPassword;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Register;
using Application.Auth.Commands.ResetPassword;
using Application.Shared.Dtos.Auth;
using Application.Shared.Dtos.Users;
using Application.Shared.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator; 

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInputDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterCommand(request), ct);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginInputDto loginDto, CancellationToken ct) 
    {
        var loginResult = await _mediator.Send(new LoginCommand(loginDto), ct);

        if (loginResult.IsLockedOut)
        {
            return BadRequest("Your account is locked due to too many failed attempts. Please try again later.");
        }

        if (loginResult.Success && loginResult.User != null)
        {
            return Ok(loginResult.User);
        }

        return Unauthorized("Invalid email or password."); // 401
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest("Email is required.");

        await _mediator.Send(new ForgotPasswordCommand(dto.Email), ct);
        return Ok(new { message = "If your email is registered, you will receive a reset link." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model, CancellationToken ct)
    {
        var result = await _mediator.Send(new ResetPasswordCommand(model), ct);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();

            // Handles: [{"code":"InvalidToken","description":"Invalid token."}]
            if (error?.Code == "InvalidToken")
            {
                return BadRequest(new { message = "This reset link is invalid or has already been used." });
            }

            // Handles validation or generic errors
            return BadRequest(new { message = error?.Description ?? "An error occurred." });
        }

        return Ok(new { message = "Password updated successfully!" });
    }
}