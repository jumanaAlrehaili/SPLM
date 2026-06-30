using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Lookups
{
    public static class RequestStatusesLookup
    {
        public static readonly RequestStatusesInfo Pending = new(1, "Pending");
        public static readonly RequestStatusesInfo Approved = new(2, "Approved");
        public static readonly RequestStatusesInfo Rejected = new(3, "Rejected");

        public static IEnumerable<RequestStatusesInfo> All =>
        [
            Pending, Approved, Rejected
        ];
    }

    public record RequestStatusesInfo(int Id, string Name);
}

