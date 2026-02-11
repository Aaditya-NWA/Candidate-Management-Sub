using System.Diagnostics.CodeAnalysis;

namespace InterviewService.DTOs;

[ExcludeFromCodeCoverage]
public class UpdateFeedbackRequest
{
  
    public string Comments { get; set; } = string.Empty;
}