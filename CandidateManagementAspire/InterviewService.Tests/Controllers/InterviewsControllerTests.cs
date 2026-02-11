using InterviewService.Controllers;
using InterviewService.DTOs;
using InterviewService.Models;
using InterviewService.Services;
using InterviewService.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

[TestFixture]
[ExcludeFromCodeCoverage]
public class InterviewsControllerTests
{
    [Test]
    public async Task GetById_NotFound_Returns404()
    {
        var db = TestDbContextFactory.Create(nameof(GetById_NotFound_Returns404));
        var service = new InterviewService.Services.InterviewService(
            db,
            new InterviewValidationService(db));

        var controller = new InterviewsController(service);

        var result = await controller.GetById(1);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Complete_DomainException_ReturnsBadRequest()
    {
        var db = TestDbContextFactory.Create(nameof(Complete_DomainException_ReturnsBadRequest));

        var interview = new Interview { Status = InterviewStatus.Completed };
        db.Interviews.Add(interview);
        await db.SaveChangesAsync();

        var controller = new InterviewsController(
            new InterviewService.Services.InterviewService(
                db,
                new InterviewValidationService(db)));

        var result = await controller.Complete(interview.Id);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public async Task Create_Valid_ReturnsCreated()
    {
        var db = TestDbContextFactory.Create(nameof(Create_Valid_ReturnsCreated));
        var controller = new InterviewsController(
            new InterviewService.Services.InterviewService(
                db,
                new InterviewValidationService(db)));

        var result = await controller.Create(new CreateInterviewRequest
        {
            CandidateId = 1,
            Project = "Payments",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            ClientInterviewRequired = false
        });

        Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
    }
    [Test]
    public async Task Create_DomainException_ReturnsBadRequest()
    {
        var db = TestDbContextFactory.Create(nameof(Create_DomainException_ReturnsBadRequest));

        // Existing interview to violate 6-month rule
        db.Interviews.Add(new Interview
        {
            CandidateId = 1,
            Project = "Payments",
            InterviewDate = DateTime.UtcNow.AddMonths(-1),
            InterviewLevel = InterviewLevel.Internal
        });
        db.SaveChanges();

        var controller = new InterviewsController(
            new InterviewService.Services.InterviewService(
                db,
                new InterviewValidationService(db)));

        var result = await controller.Create(new CreateInterviewRequest
        {
            CandidateId = 1,
            Project = "Payments",
            InterviewDate = DateTime.UtcNow,
            InterviewLevel = InterviewLevel.Internal,
            ClientInterviewRequired = false
        });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public async Task SetOutcome_Valid_ReturnsNoContent()
    {
        var db = TestDbContextFactory.Create(nameof(SetOutcome_Valid_ReturnsNoContent));

        var interview = new Interview { Status = InterviewStatus.Completed };
        db.Interviews.Add(interview);
        db.Feedbacks.Add(new Feedback { InterviewId = interview.Id, Comments = "OK" });
        await db.SaveChangesAsync();

        var controller = new InterviewsController(
            new InterviewService.Services.InterviewService(
                db,
                new InterviewValidationService(db)));

        var result = await controller.SetOutcome(
            interview.Id,
            new SetOutcomeRequest
            {
                Result = InterviewResult.Selected,
                DecisionBy = DecisionBy.Internal
            });

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task GetById_Existing_ReturnsOk()
    {
        var db = TestDbContextFactory.Create(nameof(GetById_Existing_ReturnsOk));

        var interview = new Interview { Status = InterviewStatus.Scheduled };
        db.Interviews.Add(interview);
        await db.SaveChangesAsync();

        var controller = new InterviewsController(
            new InterviewService.Services.InterviewService(
                db,
                new InterviewValidationService(db)));

        var result = await controller.GetById(interview.Id);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }
    





}
