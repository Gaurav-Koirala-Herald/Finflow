using FinFlowAPI.Data;
using FinFlowAPI.DTO.Posts;
using FinFlowAPI.Enum;
using FinFlowAPI.Models;
using FinFlowAPI.Services.Interactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult> ToggleLike(int postId, string userId)
    {
        var toggleLike = await _interactionService.LikePostAsync(postId, userId);
        return Ok(toggleLike);
    }

    [HttpPost("share")]
    public async Task<ActionResult> Share(int postId, string userId)
    {
        var share =await  _interactionService.SharePostAsync(postId, userId);
        if(share) return Ok();
        return BadRequest();
    }
}