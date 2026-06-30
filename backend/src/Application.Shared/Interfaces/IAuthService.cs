using Application.Shared.Dtos.Auth;
using Application.Shared.Dtos.Role;
using Application.Shared.Dtos.Users;
using Application.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterOutputDto> RegisterAsync(RegisterInputDto request);
        Task<IEnumerable<RoleDto>> GetRolesAsync();
        Task<LoginOutputDto> LoginAsync(LoginInputDto loginDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
