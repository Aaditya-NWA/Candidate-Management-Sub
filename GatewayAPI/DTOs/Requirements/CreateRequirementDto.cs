namespace GatewayAPI.DTOs.Requirements
{
    public class CreateRequirementDto
    {
        public string Project { get; set; } = string.Empty;
        public string SkillsNeeded { get; set; } = string.Empty;
        public string ExperienceRange { get; set; } = string.Empty;
        public string AvailabilityWindow { get; set; } = string.Empty;
        public bool ClientInterviewRequired { get; set; }
    }
}
