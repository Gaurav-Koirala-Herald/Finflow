

using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.DTO.Posts;
using FinFlowAPI.Enum;
using FinFlowAPI.Models;
using FinFlowAPI.Services.Comments;
using FinFlowAPI.Services.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    } 

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetPosts()
    {
        var allPosts = await _postsService.GetAllPostsAsync();
        return Ok(allPosts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetPost(int id)
    {
        var post = await _postsService.GetPostByIdAsync(id);
        return Ok(post);
    }
    

    [HttpPost]
    public async Task<ActionResult> CreatePost(CreatePostsDTO dto)
    {
        var response = await _postsService.CreatePostAsync(dto);
        return Ok(response);
    }
}
