using FinFlowAPI.Services.Interactions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/posts")]
public class InteractionsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    
    public InteractionsController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpPost("like")]
    public async Task<ActionResult> ToggleLike([FromQuery] int postId, [FromQuery] string userId)
    {
        var isLiked = await _interactionService.LikePostAsync(postId, userId);
        return Ok(isLiked);
    }

    [HttpPost("share")]
    public async Task<ActionResult> Share([FromQuery] int postId, [FromQuery] string userId)
    {
        var success = await _interactionService.SharePostAsync(postId, userId);
        if (success) return Ok();
        return BadRequest();
    }

    [HttpGet("{postId:int}/user/{userId}/liked")]
    public async Task<ActionResult<bool>> HasUserLiked(int postId, string userId)
    {
        var hasLiked = await _interactionService.HasUserLikedAsync(postId, userId);
        return Ok(hasLiked);
    }

    [HttpGet("{postId:int}/user/{userId}/shared")]
    public async Task<ActionResult<bool>> HasUserShared(int postId, string userId)
    {
        var hasShared = await _interactionService.HasUserSharedAsync(postId, userId);
        return Ok(hasShared);
    }
}