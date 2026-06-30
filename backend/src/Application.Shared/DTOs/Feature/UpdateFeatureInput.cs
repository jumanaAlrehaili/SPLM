using System.ComponentModel.DataAnnotations;

namespace Application.Shared.DTOs.Feature
{
    public class UpdateFeatureInput
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? EpicLink { get; set; }

        [Required]
        public int Priority { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
