using Domain.Common;

namespace Domain.Entities.Holidays
{
    public class Holiday : AuditedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Year { get; set; }
    }
}
