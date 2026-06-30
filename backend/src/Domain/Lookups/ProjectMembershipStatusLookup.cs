using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Lookups
{
    public static class ProjectMembershipStatusLookup
    {
        public static readonly ProjectMembershipStatusInfo None = new(1, "None");
        public static readonly ProjectMembershipStatusInfo Pending = new(2, "Pending");
        public static readonly ProjectMembershipStatusInfo Member = new(3, "Member");
        public static readonly ProjectMembershipStatusInfo Owner = new(4, "Owner");

        public static IEnumerable<ProjectMembershipStatusInfo> All =>
        [
            None, Pending, Member, Owner, 
        ];
    }

    public record ProjectMembershipStatusInfo(int Id, string Name);
}
