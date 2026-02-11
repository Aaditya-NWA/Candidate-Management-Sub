using InterviewService.Data;
using InterviewService.Exceptions;
using InterviewService.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.Services;

public class InterviewValidationService
{
    private readonly InterviewDbContext _context;

    public InterviewValidationService(InterviewDbContext context)
    {
        _context = context;
    }

    // Rule 1: 6-month same candidate + same project rule
    public async Task ValidateSixMonthRuleAsync(
        int candidateId,
        string project,
        DateTime interviewDate)
    {
        var sixMonthsAgo = interviewDate.AddMonths(-6);

        var exists = await _context.Interviews.AnyAsync(i =>
            i.CandidateId == candidateId &&
            i.Project == project &&
            i.InterviewDate >= sixMonthsAgo);

        if (exists)
        {
            throw new DomainValidationException(
                "Candidate has already appeared for this project in the last 6 months.");
        }
    }

    // Rule 2: Interview level constraints
    public void ValidateInterviewLevel(
        InterviewLevel interviewLevel,
        bool clientInterviewRequired,
        bool isFirstInterview)
    {
        if (isFirstInterview && interviewLevel != InterviewLevel.Internal)
        {
            throw new DomainValidationException(
                "First interview must be an internal interview.");
        }

        if (interviewLevel == InterviewLevel.Client && !clientInterviewRequired)
        {
            throw new DomainValidationException(
                "Client interview is not allowed for this requirement.");
        }
    }
}
