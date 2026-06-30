using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Lookups
{
    public static class ReleaseStageChangeTypeLookup
    {
        public static readonly ReleaseStageChangeTypeInfo Created    = new(1, "Created");
        public static readonly ReleaseStageChangeTypeInfo Updated    = new(2, "Updated");
        public static readonly ReleaseStageChangeTypeInfo Started    = new(3, "Started");
        public static readonly ReleaseStageChangeTypeInfo Completed  = new(4, "Completed");
        public static readonly ReleaseStageChangeTypeInfo Delayed    = new(5, "Delayed");
        public static readonly ReleaseStageChangeTypeInfo Reopened   = new(6, "Reopened");
        public static readonly ReleaseStageChangeTypeInfo Cancelled  = new(7, "Cancelled");

        public static IEnumerable<ReleaseStageChangeTypeInfo> All =>
        [
            Created, Updated, Started, Completed, Delayed, Reopened, Cancelled
        ];
    }

    public record ReleaseStageChangeTypeInfo(int Id, string Name);
}