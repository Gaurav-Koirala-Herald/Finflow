namespace FinFlowAPI.Models;

public class PostInteraction
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; } = "";
    public InteractionType Type { get; set; } = InteractionType.Like;

    public Post Post { get; set; } = null!;
}