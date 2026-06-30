using System.ComponentModel.DataAnnotations;

namespace Application.Shared.DTOs.Holiday
{
    public class CreateHolidayInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }

    public class UpdateHolidayInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }
    }

    public record HolidayOutput
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public int Year { get; init; }
    }
}
