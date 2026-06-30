using Microsoft.AspNetCore.Identity;

namespace Application.Shared.Dtos.Auth;

public class RegisterInputDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    //public virtual IdentityUserRole<int> RoleId { get; set; }

}
