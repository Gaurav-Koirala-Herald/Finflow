namespace FinFlowAPI.DTO.Posts;

public class CreatePostsDTO
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string UserId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}