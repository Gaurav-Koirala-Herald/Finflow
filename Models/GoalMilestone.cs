using System.ComponentModel.DataAnnotations;
using Models;

public class GoalMilestone
{

    public int Id { get; set; }
    public int GoalId { get; set; }
    [Required]
    public int Percentage { get; set; } 
    public bool IsAchieved { get; set; } = false;
    public DateTime? AchievedAt { get; set; }
    public bool NotificationSent { get; set; } = false;
    public Goal Goal { get; set; } = null!;
}