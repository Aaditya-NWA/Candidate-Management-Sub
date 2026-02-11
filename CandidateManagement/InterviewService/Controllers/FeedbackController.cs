using InterviewService.DTOs;
using InterviewService.Exceptions;
using InterviewService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewService.Controllers;

[ApiController]
[Route("api/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly FeedbackService _service;

    public FeedbackController(FeedbackService service)
    {
        _service = service;
    }

    [HttpPost("interview/{interviewId}")]
    public async Task<IActionResult> Add(int interviewId, CreateFeedbackRequest request)
    {
        try
        {
            var feedback = await _service.AddFeedbackAsync(interviewId, request.Comments);
            return Ok(feedback);
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{feedbackId}")]
    public async Task<IActionResult> Update(int feedbackId, UpdateFeedbackRequest request)
    {
        try
        {
            var feedback = await _service.UpdateFeedbackAsync(feedbackId, request.Comments);
            return Ok(feedback);
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
