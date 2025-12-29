using FinFlowAPI.DTO;
using FinFlowAPI.DTO.Posts;

namespace FinFlowAPI.Services.Posts;

public interface IPostsService
{
    Task<PostsDTO> GetPostByIdAsync(int id);
    
    Task<PostsDTO> CreatePostAsync(CreatePostsDTO dto);
    Task<List<PostsDTO>> GetAllPostsAsync();
}