using System.ComponentModel.DataAnnotations;

namespace RequirementService.DTOs
{
    public class CreateRequirementDto
    {
        [Required]
        public string Project { get; set; } = string.Empty;

        [Required]
        public string SkillsNeeded { get; set; } = string.Empty;

        [Required]
        public string ExperienceRange { get; set; } = string.Empty; // "4,15"

        [Required]
        public string AvailabilityWindow { get; set; } = string.Empty; // "2026-03-01,2026-06-01"

        public bool ClientInterviewRequired { get; set; }
    }
}
