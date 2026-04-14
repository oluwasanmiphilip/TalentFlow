// File Path: TalentFlow.API/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Dashboard.Queries;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireLearner")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var learnerId = User.FindFirst("learner_id")?.Value;
            if (learnerId == null)
                return Unauthorized(ApiResponse<string>.Fail("Unauthorized access", 401));

            var dashboard = await _mediator.Send(new GetDashboardDataQuery(learnerId));
            return Ok(ApiResponse<object>.Success(dashboard, "Dashboard retrieved successfully"));
        }
    }
}
