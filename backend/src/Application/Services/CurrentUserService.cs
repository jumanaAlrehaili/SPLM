using Application.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId => GetIntClaim("userId");

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");

        public int RoleId => GetIntClaim("roleId");

        public string? RoleName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("role")
                            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        private int GetIntClaim(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(claimType);
            return int.TryParse(claim, out var result) ? result : 0;
        }
    }
}
