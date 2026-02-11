using System.ComponentModel.DataAnnotations;

namespace RequirementService.Models;

public class Requirement
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string SkillSet { get; set; } = string.Empty;

    [Range(0, 600)]
    public int ExperienceMonths { get; set; }

    [Range(1, 1000)]
    public int OpenPositions { get; set; }

    public string Status { get; set; } = "Open"; // Open / Closed
}
