using FinFlowAPI.Data;
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
    
    public async Task<bool> LikePostAsync(int postId, string userId)
    {
        var existing = await _db.PostInteractions.SingleOrDefaultAsync(i => 
            i.PostId == postId &&
            i.UserId == userId &&
            i.Type == InteractionType.Like);

        if (existing == null)
        {
            _db.PostInteractions.Add(new PostInteraction
            {
                PostId = postId,
                UserId = userId,
                Type = InteractionType.Like
            });
        }
        else
        {
            _db.PostInteractions.Remove(existing);
        }

        await _db.SaveChangesAsync();
        return existing == null; 
    }

    public async Task<bool> SharePostAsync(int postId, string userId)
    {
        var existing = await _db.PostInteractions
            .FirstOrDefaultAsync(i => i.PostId == postId &&
                                      i.UserId == userId && 
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

        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> BookmarkPostAsync(int postId, string userId)
    {
        var existing = await _db.PostInteractions
            .FirstOrDefaultAsync(i => i.PostId == postId &&
                                      i.UserId == userId && 
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

        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> HasUserLikedAsync(int postId, string userId)
    {
        return await _db.PostInteractions
            .AnyAsync(i => i.PostId == postId && 
                          i.UserId == userId && 
                          i.Type == InteractionType.Like);
    }

    public async Task<bool> HasUserSharedAsync(int postId, string userId)
    {
        return await _db.PostInteractions
            .AnyAsync(i => i.PostId == postId && 
                          i.UserId == userId && 
                          i.Type == InteractionType.Share);
    }
}