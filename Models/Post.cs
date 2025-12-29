namespace FinFlowAPI.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string UserId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostInteraction> Interactions { get; set; } = new List<PostInteraction>();
}