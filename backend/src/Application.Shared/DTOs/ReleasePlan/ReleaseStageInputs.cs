using System.ComponentModel.DataAnnotations;

namespace Application.Shared.DTOs.ReleasePlan
{
    public class CreateReleaseStageInput
    {
        // Which catalog stage this is (BA, SA, UX/UI, Dev, QA, UAT).
        [Required]
        public int StageId { get; set; }

        // Optional custom display name; falls back to the catalog stage name when omitted.
        [MaxLength(100)]
        public string? StageName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Working days must be at least 1.")]
        public int WorkingDays { get; set; }
    }

    public class UpdateReleaseStageInput
    {
        [MaxLength(100)]
        public string? StageName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Working days must be at least 1.")]
        public int WorkingDays { get; set; }
    }
}
