using InterviewService.Data;
using InterviewService.Exceptions;
using InterviewService.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Services;

public class InterviewService
{
    private readonly InterviewDbContext _context;
    private readonly InterviewValidationService _validationService;

    public InterviewService(
        InterviewDbContext context,
        InterviewValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
    }

    public async Task<Interview> ScheduleInterviewAsync(
        Interview interview,
        bool clientInterviewRequired)
    {
        // Check if this is first interview for candidate + project
        var isFirstInterview = !await _context.Interviews.AnyAsync(i =>
            i.CandidateId == interview.CandidateId &&
            i.Project == interview.Project);

        // Validation rules
        await _validationService.ValidateSixMonthRuleAsync(
            interview.CandidateId,
            interview.Project,
            interview.InterviewDate);

        _validationService.ValidateInterviewLevel(
            interview.InterviewLevel,
            clientInterviewRequired,
            isFirstInterview);

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();

        return interview;
    }

    public async Task CompleteInterviewAsync(int interviewId)
    {
        var interview = await _context.Interviews.FindAsync(interviewId);

        if (interview == null)
            throw new DomainValidationException("Interview not found.");

        if (interview.Status == InterviewStatus.Completed)
            throw new DomainValidationException("Interview is already completed.");

        interview.Status = InterviewStatus.Completed;

        await _context.SaveChangesAsync();
    }


    public async Task<Interview?> GetByIdAsync(int id)
    {
        return await _context.Interviews.FindAsync(id);
    }

    public async Task<List<Interview>> GetByCandidateAsync(int candidateId)
    {
        return await _context.Interviews
            .Where(i => i.CandidateId == candidateId)
            .ToListAsync();
    }
    [ExcludeFromCodeCoverage]

    public async Task SetOutcomeAsync(
    int interviewId,
    InterviewResult result,
    DecisionBy decisionBy)
    {
        var interview = await _context.Interviews.FindAsync(interviewId);

        if (interview == null)
            throw new DomainValidationException("Interview not found.");

        if (interview.Status != InterviewStatus.Completed)
            throw new DomainValidationException("Outcome can only be set after interview completion.");

        if (interview.Result.HasValue)
            throw new DomainValidationException("Outcome already set for this interview.");

        var feedbackExists = await _context.Feedbacks
            .AnyAsync(f => f.InterviewId == interviewId);

        if (!feedbackExists)
            throw new DomainValidationException("Feedback is required before setting outcome.");

        interview.Result = result;
        interview.DecisionBy = decisionBy;

        await _context.SaveChangesAsync();
    }

}
