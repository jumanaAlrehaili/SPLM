using Application.Shared.Dtos.Auth;
using Application.Shared.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest<IdentityResult>;