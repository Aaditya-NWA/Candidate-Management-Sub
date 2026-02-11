using InterviewService.Data;
using InterviewService.Exceptions;
using InterviewService.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.Services;

public class FeedbackService
{
    private readonly InterviewDbContext _context;

    public FeedbackService(InterviewDbContext context)
    {
        _context = context;
    }

    public async Task<Feedback> AddFeedbackAsync(int interviewId, string comments)
    {
        var interview = await _context.Interviews.FindAsync(interviewId);

        if (interview == null)
            throw new DomainValidationException("Interview not found.");

        if (interview.Status != InterviewStatus.Completed)
            throw new DomainValidationException("Feedback can only be added after interview completion.");

        var existing = await _context.Feedbacks
            .AnyAsync(f => f.InterviewId == interviewId);

        if (existing)
            throw new DomainValidationException("Feedback already exists for this interview.");

        var feedback = new Feedback
        {
            InterviewId = interviewId,
            Comments = comments
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        return feedback;
    }

    public async Task<Feedback> UpdateFeedbackAsync(int feedbackId, string comments)
    {
        var feedback = await _context.Feedbacks.FindAsync(feedbackId);

        if (feedback == null)
            throw new DomainValidationException("Feedback not found.");

        feedback.Comments = comments;
        feedback.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return feedback;
    }

    public async Task<Feedback?> GetByInterviewAsync(int interviewId)
    {
        return await _context.Feedbacks
            .FirstOrDefaultAsync(f => f.InterviewId == interviewId);
    }
}
