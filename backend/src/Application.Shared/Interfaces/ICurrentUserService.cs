using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.Interfaces
{
    public interface ICurrentUserService
    {
        // Access the current user ID from the JWT Claims
        int UserId { get; }
        string? Email { get; }
        int RoleId { get; }
        string? RoleName { get; }
    }
}
