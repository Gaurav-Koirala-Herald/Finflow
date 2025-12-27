using System.Text.Json.Serialization;

namespace FinFlowAPI.Models;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string AuthorId { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Post Post { get; set; } = null!;
}