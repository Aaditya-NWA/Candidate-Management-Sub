using InterviewService.Exceptions;
using InterviewService.Models;
using InterviewService.Services;
using InterviewService.Tests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Tests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class FeedbackServiceTests
{
    [Test]
    public async Task AddFeedback_AfterCompletion_ShouldSucceed()
    {
        var context = TestDbContextFactory.Create(
            nameof(AddFeedback_AfterCompletion_ShouldSucceed));

        var interview = new Interview
        {
            CandidateId = 1,
            Project = "PaymentsPlatform",
            Status = InterviewStatus.Completed
        };

        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        var service = new FeedbackService(context);

        var feedback = await service.AddFeedbackAsync(
            interview.Id,
            "Good candidate");

        Assert.That(feedback.Comments, Is.EqualTo("Good candidate"));
    }
    [Test]
    public async Task AddFeedback_InterviewNotCompleted_Throws()
    {
        var db = TestDbContextFactory.Create(nameof(AddFeedback_InterviewNotCompleted_Throws));
        db.Interviews.Add(new Interview { Status = InterviewStatus.Scheduled });
        db.SaveChanges();

        var service = new FeedbackService(db);

        Assert.ThrowsAsync<DomainValidationException>(() =>
            service.AddFeedbackAsync(1, "test"));
    }
    [Test]
    public async Task GetByInterviewAsync_WhenExists_ReturnsFeedback()
    {
        var context = TestDbContextFactory.Create(
            nameof(GetByInterviewAsync_WhenExists_ReturnsFeedback));

        var interview = new Interview { Status = InterviewStatus.Completed };
        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        var feedback = new Feedback
        {
            InterviewId = interview.Id,
            Comments = "ok"
        };
        context.Feedbacks.Add(feedback);
        await context.SaveChangesAsync();

        var service = new FeedbackService(context);

        var result = await service.GetByInterviewAsync(interview.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Comments, Is.EqualTo("ok"));
    }

    [Test]
    public async Task UpdateFeedbackAsync_Valid_UpdatesComments()
    {
        var db = TestDbContextFactory.Create(nameof(UpdateFeedbackAsync_Valid_UpdatesComments));

        var feedback = new Feedback { InterviewId = 1, Comments = "Old" };
        db.Feedbacks.Add(feedback);
        await db.SaveChangesAsync();

        var service = new FeedbackService(db);

        var updated = await service.UpdateFeedbackAsync(feedback.Id, "New");

        Assert.That(updated.Comments, Is.EqualTo("New"));
    }
    [Test]
    public void AddFeedback_InterviewNotFound_Throws()
    {
        var context = TestDbContextFactory.Create(
            nameof(AddFeedback_InterviewNotFound_Throws));

        var service = new FeedbackService(context);

        Assert.ThrowsAsync<DomainValidationException>(() =>
            service.AddFeedbackAsync(999, "test"));
    }

    [Test]
    public async Task AddFeedback_WhenFeedbackAlreadyExists_Throws()
    {
        var context = TestDbContextFactory.Create(
            nameof(AddFeedback_WhenFeedbackAlreadyExists_Throws));

        var interview = new Interview
        {
            Status = InterviewStatus.Completed
        };

        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        context.Feedbacks.Add(new Feedback
        {
            InterviewId = interview.Id,
            Comments = "existing"
        });
        await context.SaveChangesAsync();

        var service = new FeedbackService(context);

        Assert.ThrowsAsync<DomainValidationException>(() =>
            service.AddFeedbackAsync(interview.Id, "new"));
    }

    

    [Test]
    public async Task GetByInterviewAsync_WhenMissing_ReturnsNull()
    {
        var context = TestDbContextFactory.Create(
            nameof(GetByInterviewAsync_WhenMissing_ReturnsNull));

        var service = new FeedbackService(context);

        var result = await service.GetByInterviewAsync(123);

        Assert.That(result, Is.Null);
    }


}
