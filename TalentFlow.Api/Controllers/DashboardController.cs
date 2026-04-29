// File Path: TalentFlow.API/Controllers/DashboardController.cs

using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Common.Models;
using TalentFlow.Application.Dashboard.Queries;

namespace TalentFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Policy = "RequireLearner")]
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
            if (string.IsNullOrWhiteSpace(learnerId))
                return Unauthorized(ApiResponse.Fail<string>("Unauthorized access", 401));

            var dashboard = await _mediator.Send(new GetDashboardDataQuery(learnerId));
            if (dashboard == null)
                return NotFound(ApiResponse.Fail<string>("Dashboard data not found", 404));

            return Ok(ApiResponse.Success<object>(dashboard, "Dashboard retrieved successfully"));
        }
    }
}
