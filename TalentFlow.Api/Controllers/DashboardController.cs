using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireLearner")]
public class DashboardController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDashboard()
    {
        var learnerId = User.FindFirst("learner_id")?.Value;
        if (learnerId == null)
            return Unauthorized(ApiResponse<string>.Fail("Unauthorized access", 401));

        return Ok(ApiResponse<object>.Success(new
        {
            learner_id = learnerId,
            courses = new[] { "Course A", "Course B" }
        }, "Dashboard retrieved successfully"));
    }
}
