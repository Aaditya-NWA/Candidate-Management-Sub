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
public class FeedbackControllerTests
{
    [Test]
    public async Task Add_Valid_ReturnsOk()
    {
        var db = TestDbContextFactory.Create(nameof(Add_Valid_ReturnsOk));

        var interview = new Interview { Status = InterviewStatus.Completed };
        db.Interviews.Add(interview);
        await db.SaveChangesAsync();

        var service = new FeedbackService(db);
        var controller = new FeedbackController(service);

        var result = await controller.Add(
            interview.Id,
            new CreateFeedbackRequest { Comments = "Good" });

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Add_DomainException_ReturnsBadRequest()
    {
        var db = TestDbContextFactory.Create(nameof(Add_DomainException_ReturnsBadRequest));

        db.Interviews.Add(new Interview { Status = InterviewStatus.Scheduled });
        await db.SaveChangesAsync();

        var controller = new FeedbackController(new FeedbackService(db));

        var result = await controller.Add(
            1,
            new CreateFeedbackRequest { Comments = "Bad" });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public async Task Update_DomainException_ReturnsBadRequest()
    {
        var db = TestDbContextFactory.Create(nameof(Update_DomainException_ReturnsBadRequest));

        var service = new FeedbackService(db);
        var controller = new FeedbackController(service);

        var result = await controller.Update(
            999,
            new UpdateFeedbackRequest { Comments = "x" });

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public async Task Update_Valid_ReturnsOk()
    {
        var db = TestDbContextFactory.Create(nameof(Update_Valid_ReturnsOk));

        var feedback = new Feedback { InterviewId = 1, Comments = "Old" };
        db.Feedbacks.Add(feedback);
        await db.SaveChangesAsync();

        var controller = new FeedbackController(new FeedbackService(db));

        var result = await controller.Update(
            feedback.Id,
            new UpdateFeedbackRequest { Comments = "Updated" });

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }



}
