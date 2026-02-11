using System.Diagnostics.CodeAnalysis;

namespace InterviewService.DTOs;

[ExcludeFromCodeCoverage]
public class CreateFeedbackRequest
{
    
    public string Comments { get; set; } = string.Empty;
}
