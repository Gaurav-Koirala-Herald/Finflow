using FinFlowAPI.DTO.Posts;

namespace FinFlowAPI.Services.Interactions;

public interface IInteractionService
{
    Task<bool> LikePostAsync(int postId, string userId);
    Task<bool> SharePostAsync(int postId, string userId);
    Task<bool> BookmarkPostAsync(int postId, string userId);
    Task<bool> HasUserLikedAsync(int postId, string userId);
    Task<bool> HasUserSharedAsync(int postId, string userId);
}