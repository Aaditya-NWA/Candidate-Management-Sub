using InterviewService.Models;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.DTOs;

[ExcludeFromCodeCoverage]
public class CreateInterviewRequest
{
   
    public int CandidateId { get; set; }

    public string Project { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string Interviewer { get; set; } = string.Empty;

    public DateTime InterviewDate { get; set; }

    public InterviewLevel InterviewLevel { get; set; }

    public bool ClientInterviewRequired { get; set; }
}
