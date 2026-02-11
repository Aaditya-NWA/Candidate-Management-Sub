using InterviewService.Exceptions;
using InterviewService.Models;
using InterviewService.Services;
using InterviewService.Tests.Helpers;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace InterviewService.Tests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class InterviewValidationServiceTests
{
    [Test]
    public async Task ScheduleInterview_ShouldCreateInterview()
    {
        var context = TestDbContextFactory.Create(
            nameof(ScheduleInterview_ShouldCreateInterview));

        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(
            context,
            validationService);

        var interview = new Interview
        {
            CandidateId = 10,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal
        };

        var result = await service.ScheduleInterviewAsync(
            interview,
            clientInterviewRequired: false);

        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Status, Is.EqualTo(InterviewStatus.Scheduled));
    }
    [Test]
    public async Task CompleteInterview_ShouldMarkCompleted()
    {
        var context = TestDbContextFactory.Create(
            nameof(CompleteInterview_ShouldMarkCompleted));

        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(
            context,
            validationService);

        var interview = new Interview
        {
            CandidateId = 11,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            Status = InterviewStatus.Scheduled
        };

        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        await service.CompleteInterviewAsync(interview.Id);

        var updated = context.Interviews.Single();
        Assert.That(updated.Status, Is.EqualTo(InterviewStatus.Completed));
    }
    [Test]
    public async Task SetOutcome_ShouldSetResultAndDecision()
    {
        var context = TestDbContextFactory.Create(
            nameof(SetOutcome_ShouldSetResultAndDecision));

        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(
            context,
            validationService);

        var interview = new Interview
        {
            CandidateId = 12,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            Status = InterviewStatus.Completed
        };

        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        // 🔑 ADD FEEDBACK FIRST
        var feedbackService = new FeedbackService(context);
        await feedbackService.AddFeedbackAsync(interview.Id, "Good performance");

        await service.SetOutcomeAsync(
    interview.Id,
    InterviewResult.Selected,
    DecisionBy.Internal);

        var updated = context.Interviews.Single();
        Assert.That(updated.Result, Is.EqualTo(InterviewResult.Selected));
        Assert.That(updated.DecisionBy, Is.EqualTo(DecisionBy.Internal));


    }

    [Test]
    [ExcludeFromCodeCoverage]
    public async Task ScheduleInterview_InternalFirst_WhenClientRequired_ShouldPass()
    {
        var context = TestDbContextFactory.Create(nameof(ScheduleInterview_InternalFirst_WhenClientRequired_ShouldPass));
        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(context, validationService);

        var interview = new Interview
        {
            CandidateId = 20,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal
        };

        var result = await service.ScheduleInterviewAsync(interview, clientInterviewRequired: true);

        Assert.That(result.Status, Is.EqualTo(InterviewStatus.Scheduled));
    }

    [Test]
    public void DuplicateInterviewWithinSixMonths_ShouldThrow()
    {
        var context = TestDbContextFactory.Create(
            nameof(DuplicateInterviewWithinSixMonths_ShouldThrow));

        context.Interviews.Add(new Interview
        {
            CandidateId = 1,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow.AddMonths(-2),
            InterviewLevel = InterviewLevel.Internal
        });

        context.SaveChanges();

        var service = new InterviewValidationService(context);

        Assert.Throws<DomainValidationException>(() =>
            service.ValidateSixMonthRuleAsync(
                1,
                "PaymentsPlatform",
                DateTime.UtcNow
            ).GetAwaiter().GetResult());
    }
    [Test]
    public void ScheduleInterview_ClientInterviewAsFirstLevel_ShouldThrow()
    {
        var context = TestDbContextFactory.Create(
            nameof(ScheduleInterview_ClientInterviewAsFirstLevel_ShouldThrow));

        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(context, validationService);

        var interview = new Interview
        {
            CandidateId = 20,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Client // ❌ client first
        };

        Assert.ThrowsAsync<DomainValidationException>(async () =>
            await service.ScheduleInterviewAsync(interview, clientInterviewRequired: true));
    }

    
    [Test]
    public void CompleteInterview_AlreadyCompleted_ShouldThrow()
    {
        var context = TestDbContextFactory.Create(
            nameof(CompleteInterview_AlreadyCompleted_ShouldThrow));

        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(context, validationService);

        var interview = new Interview
        {
            CandidateId = 30,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            Status = InterviewStatus.Completed
        };

        context.Interviews.Add(interview);
        context.SaveChanges();

        Assert.ThrowsAsync<DomainValidationException>(async () =>
    await service.CompleteInterviewAsync(interview.Id));




    }
    [Test]
    public void SetOutcome_WhenInterviewNotCompleted_ShouldThrow()
    {
        var context = TestDbContextFactory.Create(
            nameof(SetOutcome_WhenInterviewNotCompleted_ShouldThrow));

        var validationService = new InterviewValidationService(context);
        var service = new InterviewService.Services.InterviewService(context, validationService);

        var interview = new Interview
        {
            CandidateId = 40,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            Status = InterviewStatus.Scheduled
        };

        context.Interviews.Add(interview);
        context.SaveChanges();

        Assert.ThrowsAsync<DomainValidationException>(async () =>
    await service.SetOutcomeAsync(
        interview.Id,
        InterviewResult.Selected,
        DecisionBy.Internal));

    }
    [Test]
    public async Task AddFeedback_ShouldPersistFeedback()
    {
        var context = TestDbContextFactory.Create(
            nameof(AddFeedback_ShouldPersistFeedback));

        var interview = new Interview
        {
            CandidateId = 50,
            Project = "PaymentsPlatform",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            Status = InterviewStatus.Completed
        };

        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        var service = new FeedbackService(context);

        await service.AddFeedbackAsync(interview.Id, "Strong skills");

        var saved = context.Feedbacks.Single();
        Assert.That(saved.Comments, Is.EqualTo("Strong skills"));
    }
    [Test]
    public void ValidateInterviewLevel_FirstInterviewNotInternal_Throws()
    {
        var service = new InterviewValidationService(null!);

        Assert.Throws<DomainValidationException>(() =>
            service.ValidateInterviewLevel(
                InterviewLevel.Client,
                clientInterviewRequired: true,
                isFirstInterview: true));
    }

}
