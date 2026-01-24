using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FinFlowAPI.DTO.Goals;

namespace FinFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// Get all goals for the authenticated user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetGoals()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var goals = await _goalService.GetUserGoalsAsync(userId.Value);
            return Ok(goals);
        }

        /// <summary>
        /// Get a specific goal by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalDto>> GetGoal(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var goal = await _goalService.GetGoalByIdAsync(id, userId.Value);
            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        /// <summary>
        /// Create a new goal
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GoalDto>> CreateGoal(CreateGoalDto createGoalDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate deadline is in the future
            if (createGoalDto.Deadline <= DateTime.UtcNow)
                return BadRequest("Goal deadline must be in the future.");

            var goal = await _goalService.CreateGoalAsync(createGoalDto, userId.Value);
            return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
        }

        /// <summary>
        /// Update an existing goal
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<GoalDto>> UpdateGoal(int id, UpdateGoalDto updateGoalDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate deadline is in the future if provided
            if (updateGoalDto.Deadline.HasValue && updateGoalDto.Deadline <= DateTime.UtcNow)
                return BadRequest("Goal deadline must be in the future.");

            var goal = await _goalService.UpdateGoalAsync(id, updateGoalDto, userId.Value);
            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        /// <summary>
        /// Delete a goal
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var success = await _goalService.DeleteGoalAsync(id, userId.Value);
            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Get goal progress with milestone information
        /// </summary>
        [HttpGet("{id}/progress")]
        public async Task<ActionResult<GoalProgressDto>> GetGoalProgress(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var progress = await _goalService.GetGoalProgressAsync(id, userId.Value);
            if (progress == null)
                return NotFound();

            return Ok(progress);
        }

        /// <summary>
        /// Update progress for all user goals based on transactions
        /// </summary>
        [HttpPost("update-progress")]
        public async Task<IActionResult> UpdateGoalProgress()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            await _goalService.UpdateGoalProgressAsync(userId.Value);
            return Ok(new { message = "Goal progress updated successfully" });
        }

        /// <summary>
        /// Get goals nearing their deadline
        /// </summary>
        [HttpGet("nearing-deadline")]
        public async Task<ActionResult<IEnumerable<GoalDto>>> GetGoalsNearingDeadline([FromQuery] int daysThreshold = 30)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (daysThreshold < 1 || daysThreshold > 365)
                return BadRequest("Days threshold must be between 1 and 365.");

            var goals = await _goalService.GetGoalsNearingDeadlineAsync(userId.Value, daysThreshold);
            return Ok(goals);
        }

        /// <summary>
        /// Get milestone achievements for a specific goal
        /// </summary>
        [HttpGet("{id}/milestones")]
        public async Task<ActionResult<IEnumerable<GoalMilestoneDto>>> GetGoalMilestones(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Verify user owns the goal
            var goal = await _goalService.GetGoalByIdAsync(id, userId.Value);
            if (goal == null)
                return NotFound();

            var milestones = await _goalService.GetGoalMilestonesAsync(id);
            return Ok(milestones);
        }

        /// <summary>
        /// Get recent milestone achievements for the user
        /// </summary>
        [HttpGet("recent-achievements")]
        public async Task<ActionResult<IEnumerable<GoalMilestoneDto>>> GetRecentAchievements([FromQuery] int daysBack = 7)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (daysBack < 1 || daysBack > 90)
                return BadRequest("Days back must be between 1 and 90.");

            var achievements = await _goalService.GetRecentAchievementsAsync(userId.Value, daysBack);
            return Ok(achievements);
        }

        /// <summary>
        /// Manually trigger milestone check for a specific goal
        /// </summary>
        [HttpPost("{id}/check-milestones")]
        public async Task<IActionResult> CheckGoalMilestones(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Verify user owns the goal
            var goal = await _goalService.GetGoalByIdAsync(id, userId.Value);
            if (goal == null)
                return NotFound();

            await _goalService.CheckAndUpdateMilestonesAsync(id);
            return Ok(new { message = "Milestones checked and updated successfully" });
        }

        /// <summary>
        /// Add money to a goal manually
        /// </summary>
        [HttpPost("{id}/contribute")]
        public async Task<IActionResult> AddContribution(int id, [FromBody] AddContributionRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (request.Amount <= 0)
                return BadRequest("Contribution amount must be greater than 0");

            var success = await _goalService.AddGoalContributionAsync(id, userId.Value, request.Amount, request.Description, request.TransactionId);
            if (!success)
                return BadRequest("Failed to add contribution to goal");

            return Ok(new { message = "Contribution added successfully" });
        }

        /// <summary>
        /// Complete a goal by adding the remaining amount
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteGoal(int id, [FromBody] CompleteGoalRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var success = await _goalService.CompleteGoalAsync(id, userId.Value, request.CompletionNote);
            if (!success)
                return BadRequest("Failed to complete goal");

            return Ok(new { message = "Goal completed successfully" });
        }

        /// <summary>
        /// Get contribution history for a specific goal
        /// </summary>
        [HttpGet("{id}/contributions")]
        public async Task<ActionResult<IEnumerable<GoalContributionHistoryDto>>> GetGoalContributions(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Verify user owns the goal
            var goal = await _goalService.GetGoalByIdAsync(id, userId.Value);
            if (goal == null)
                return NotFound();

            var contributions = await _goalService.GetGoalContributionHistoryAsync(id, userId.Value);
            return Ok(contributions);
        }

        /// <summary>
        /// Add contribution from a transaction
        /// </summary>
        [HttpPost("{goalId}/contribute-from-transaction/{transactionId}")]
        public async Task<IActionResult> AddTransactionContribution(int goalId, int transactionId, [FromBody] decimal amount)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (amount <= 0)
                return BadRequest("Contribution amount must be greater than 0");

            var success = await _goalService.AddGoalContributionAsync(goalId, userId.Value, amount, $"Contribution from transaction {transactionId}", transactionId);
            if (!success)
                return BadRequest("Failed to add transaction contribution to goal");

            return Ok(new { message = "Transaction contribution added successfully" });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public class AddContributionRequest
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public int? TransactionId { get; set; }
    }

    public class CompleteGoalRequest
    {
        public string? CompletionNote { get; set; }
    }
}