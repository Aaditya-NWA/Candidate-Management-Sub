using InterviewService.Models;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.DTOs;

[ExcludeFromCodeCoverage]
public class SetOutcomeRequest
{
  
    public InterviewResult Result { get; set; }
    public DecisionBy DecisionBy { get; set; }
}
