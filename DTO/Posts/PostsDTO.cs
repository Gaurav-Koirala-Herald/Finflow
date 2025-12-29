namespace FinFlowAPI.DTO.Posts;

public class PostsDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string UserId { get; set; } 
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}