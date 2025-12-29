using FinFlowAPI.DTO;

namespace FinFlowAPI.Services.Comments;

public interface ICommentService
{
    Task<CommentDTO> CreateCommentAsync(CommentDTO dto);
    Task<bool> DeleteCommentAsync(int id);
    Task<List<CommentDTO>> GetAllCommentsAsync(int postId);
}