using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Models;

[ExcludeFromCodeCoverage]
public class Feedback
{
    
    public int Id { get; set; }

    public int InterviewId { get; set; }

    public string Comments { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Interview Interview { get; set; } = null!;
}
