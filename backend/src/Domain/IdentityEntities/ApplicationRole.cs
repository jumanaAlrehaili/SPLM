using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.IdentityEntities;

public class ApplicationRole : IdentityRole<int>
{
    public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; } = new List<IdentityUserRole<int>>();
}

