namespace FinFlowAPI.DTO;

public class CommentDTO
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}