using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentFlow.Application.Dashboard.Learner.DTOs;
using TalentFlow.Application.Dashboard.Learner.Queries;

namespace TalentFlow.Api.Controllers
{
    [ApiController]
    [Route("api/learner/dashboard")]
    [Authorize(Policy = "RequireLearner")]
    public class LearnerDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LearnerDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{learnerId}")]
        public async Task<ActionResult<LearnerDashboardDto>> Get(string learnerId)
        {
            var result = await _mediator.Send(new GetLearnerDashboardQuery(learnerId));
            return Ok(result);
        }
    }
}
