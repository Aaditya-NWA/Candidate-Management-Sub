using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Models;

[ExcludeFromCodeCoverage]
public class Interview
{
   
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public string Project { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string Interviewer { get; set; } = string.Empty;

    public DateTime InterviewDate { get; set; }

    public InterviewLevel InterviewLevel { get; set; }

    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;

    // Outcome
    public InterviewResult? Result { get; set; }

    public DecisionBy? DecisionBy { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
