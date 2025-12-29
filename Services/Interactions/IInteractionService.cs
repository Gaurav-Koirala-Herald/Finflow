using FinFlowAPI.DTO.Posts;

namespace FinFlowAPI.Services.Interactions;

public interface IInteractionService
{
    Task<PostsDTO> LikePostAsync(int postId, string userId);
    Task<bool> SharePostAsync(int postId, string userId);
    
    Task<bool> BookmarkPostAsync(int postId, string userId);
}