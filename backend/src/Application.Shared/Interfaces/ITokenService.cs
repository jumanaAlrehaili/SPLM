using System;
using System.Collections.Generic;
using System.Text;
using Domain.IdentityEntities;

namespace Application.Shared.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, int roleId, string roleName); //return token as string
                                                                       //IList<string> roles

    }
}
