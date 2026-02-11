using InterviewService.DTOs;
using InterviewService.Exceptions;
using InterviewService.Models;
using InterviewService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterviewService.Controllers;

[ApiController]
[Route("api/interviews")]
public class InterviewsController : ControllerBase
{
    private readonly InterviewService.Services.InterviewService _service;

    public InterviewsController(InterviewService.Services.InterviewService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInterviewRequest request)
    {
        try
        {
            var interview = new Interview
            {
                CandidateId = request.CandidateId,
                Project = request.Project,
                Account = request.Account,
                Interviewer = request.Interviewer,
                InterviewDate = request.InterviewDate,
                InterviewLevel = request.InterviewLevel
            };

            var created = await _service.ScheduleInterviewAsync(
                interview,
                request.ClientInterviewRequired);

            return CreatedAtAction(nameof(GetById),
                new { id = created.Id },
                created);
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            await _service.CompleteInterviewAsync(id);
            return NoContent();
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var interview = await _service.GetByIdAsync(id);
        return interview == null ? NotFound() : Ok(interview);
    }
    [HttpPost("{id}/outcome")]
    public async Task<IActionResult> SetOutcome(int id, SetOutcomeRequest request)
    {
        try
        {
            await _service.SetOutcomeAsync(id, request.Result, request.DecisionBy);
            return NoContent();
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
