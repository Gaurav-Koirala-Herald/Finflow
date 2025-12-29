
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;
using FinFlowAPI.Services.Comments;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/posts/{postId:int}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController( ICommentService commentService)
    {
        _commentService = commentService;
    }
    

    [HttpPost]
    public async Task<ActionResult> CreateComment(int postId, CommentDTO dto)
    {
       var commentReponse = await _commentService.CreateCommentAsync(dto);
       return Ok(commentReponse);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
    {
      var comments = await _commentService.GetAllCommentsAsync(postId);
       return Ok(comments);
    }
}