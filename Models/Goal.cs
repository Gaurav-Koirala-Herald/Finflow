using System.ComponentModel.DataAnnotations;
using FinFlowAPI.Models;

namespace Models
{
    public class Goal
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; } = 0;

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public GoalType Type { get; set; }

        public GoalStatus Status { get; set; } = GoalStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }

    public enum GoalType
    {
        Savings,
        Investment,
        DebtRepayment
    }

    public enum GoalStatus
    {
        Active,
        Completed,
        Paused
    }
}