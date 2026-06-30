using System.ComponentModel.DataAnnotations;

namespace Application.Shared.DTOs.ReleasePlan
{
    public class CreateReleasePlanInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class UpdateReleasePlanInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class CreateReleaseInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IncludeUIUXStage { get; set; } = true;
    }

    public class UpdateReleaseInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
