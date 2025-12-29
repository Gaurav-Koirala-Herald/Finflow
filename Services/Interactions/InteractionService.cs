using FinFlowAPI.Data;
using FinFlowAPI.DTO.Posts;
using FinFlowAPI.Enum;
using FinFlowAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FinFlowAPI.Services.Interactions;

public class InteractionService : IInteractionService
{
    private readonly ApplicationDbContext _db;
    public InteractionService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<PostsDTO> LikePostAsync(int postId, string userId)
    {
        var existing = await _db.PostInteractions.SingleOrDefaultAsync(i => i.PostId == postId &&
                                      i.UserId == userId &&
                                      i.Type == InteractionType.Like);
        bool isLiked= true;

        if (existing == null)
        {
            _db.PostInteractions.Add(new PostInteraction
            {
                PostId = postId,
                UserId = userId,
                Type = InteractionType.Like
            });
            isLiked = true;
        }
        else
        {
            _db.PostInteractions.Remove(existing);
        }

        await _db.SaveChangesAsync();
        return new PostsDTO()
        {
            
        };
    }

    public async Task<bool> SharePostAsync(int postId, string userId)
    {
        var existing = await _db.PostInteractions
            .FirstOrDefaultAsync(i => i.PostId == postId &&
                                      i.UserId .Contains(userId) &&
                                      i.Type == InteractionType.Share);

        if (existing == null)
        {
            _db.PostInteractions.Add(new PostInteraction
            {
                PostId = postId,
                UserId = userId,
                Type = InteractionType.Share
            });
        }
        else
        {
            _db.PostInteractions.Remove(existing);
        }

        return  await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> BookmarkPostAsync(int postId, string userId)
    {
        var existing = await _db.PostInteractions
            .FirstOrDefaultAsync(i => i.PostId == postId &&
                                      i.UserId .Contains(userId) &&
                                      i.Type == InteractionType.Bookmark);

        if (existing == null)
        {
            _db.PostInteractions.Add(new PostInteraction
            {
                PostId = postId,
                UserId = userId,
                Type = InteractionType.Bookmark
            });
        }
        else
        {
            _db.PostInteractions.Remove(existing);
        }

        return await _db.SaveChangesAsync() > 0 ;
    }
    
}