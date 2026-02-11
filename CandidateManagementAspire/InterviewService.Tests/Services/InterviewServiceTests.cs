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
public class InterviewServiceTests
{
    [Test]
    public async Task CompleteInterview_ShouldSetStatusCompleted()
    {
        var context = TestDbContextFactory.Create(
            nameof(CompleteInterview_ShouldSetStatusCompleted));

        var interview = new Interview
        {
            CandidateId = 1,
            Project = "PaymentsPlatform",
            InterviewLevel = InterviewLevel.Internal
        };

        context.Interviews.Add(interview);
        await context.SaveChangesAsync();

        var service = new Services.InterviewService(
            context,
            new InterviewValidationService(context));

        await service.CompleteInterviewAsync(interview.Id);

        var updated = context.Interviews.First();
        Assert.That(updated.Status, Is.EqualTo(InterviewStatus.Completed));
    }
    [Test]
    public async Task CompleteInterview_NotFound_Throws()
    {
        var db = TestDbContextFactory.Create(nameof(CompleteInterview_NotFound_Throws));
        var service = new InterviewService.Services.InterviewService(
            db, new InterviewValidationService(db));

        Assert.ThrowsAsync<DomainValidationException>(() =>
            service.CompleteInterviewAsync(99));
    }
    [Test]
    public async Task SetOutcome_NoFeedback_Throws()
    {
        var db = TestDbContextFactory.Create(nameof(SetOutcome_NoFeedback_Throws));

        var interview = new Interview { Status = InterviewStatus.Completed };
        db.Interviews.Add(interview);
        db.SaveChanges();

        var service = new InterviewService.Services.InterviewService(
            db, new InterviewValidationService(db));

        Assert.ThrowsAsync<DomainValidationException>(() =>
            service.SetOutcomeAsync(interview.Id, InterviewResult.Selected, DecisionBy.Client));
    }
    [Test]
    public async Task GetByCandidateAsync_ReturnsCandidateInterviews()
    {
        var db = TestDbContextFactory.Create(nameof(GetByCandidateAsync_ReturnsCandidateInterviews));

        db.Interviews.AddRange(
            new Interview { CandidateId = 99 },
            new Interview { CandidateId = 99 },
            new Interview { CandidateId = 1 }
        );
        await db.SaveChangesAsync();

        var service = new InterviewService.Services.InterviewService(
            db,
            new InterviewValidationService(db));

        var result = await service.GetByCandidateAsync(99);

        Assert.That(result.Count, Is.EqualTo(2));
    }



}
