using System.Xml.Serialization;
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.DTO.Posts;
using FinFlowAPI.Enum;
using FinFlowAPI.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FinFlowAPI.Services.Posts;

public class PostsService : IPostsService
{
    private readonly ApplicationDbContext _context;
    public PostsService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PostsDTO> GetPostByIdAsync(int id)
    {
        var existingPost = await _context.Posts.SingleOrDefaultAsync(x=>x.Id == id) ;
        var mappedResponse = existingPost.Adapt<PostsDTO>();
        return mappedResponse;
    }

    public async Task<PostsDTO> CreatePostAsync(CreatePostsDTO dto)
    {
        var mappedRequest = dto.Adapt<Post>();
        await _context.Posts.AddAsync(mappedRequest);
        await _context.SaveChangesAsync();
        var mappedResponse = mappedRequest.Adapt<PostsDTO>();
        return mappedResponse;
    }

    public async Task<List<PostsDTO>> GetAllPostsAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments)
            .Include(p => p.Interactions)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostsDTO()
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                UserId = p.UserId,
                CreatedAt = p.CreatedAt,
                CommentCount = p.Comments.Count,
                LikeCount = p.Interactions.Count(i => i.Type == InteractionType.Like),
                ShareCount = p.Interactions.Count(i => i.Type == InteractionType.Share)
            })
            .ToListAsync();
        var mappedResponse = posts.Adapt<List<PostsDTO>>();

        return mappedResponse;
    }
}