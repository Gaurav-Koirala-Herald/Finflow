using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FinFlowAPI.Models;
using Models;
namespace FinFlowAPI.DTO.Goals;


public class CreateGoalDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
    public decimal TargetAmount { get; set; }

    [Required]
    public DateTime Deadline { get; set; }

    [Required]
    public GoalType Type { get; set; }
}

public class UpdateGoalDto
{
    [StringLength(200, MinimumLength = 1)]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
    public decimal? TargetAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Current amount must be non-negative")]
    public decimal? CurrentAmount { get; set; }

    public DateTime? Deadline { get; set; }

    public GoalType? Type { get; set; }

    public GoalStatus? Status { get; set; }
}

public class GoalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime Deadline { get; set; }
    public GoalType Type { get; set; }
    public GoalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal ProgressPercentage { get; set; }
    public int DaysRemaining { get; set; }
}

public class GoalProgressDto
{
    public int GoalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal ProgressPercentage { get; set; }
    public string Status { get; set; }
    public List<MilestoneDto> Milestones { get; set; } = new();
}

public class MilestoneDto
{
    public int Percentage { get; set; }
    public bool IsAchieved { get; set; }
    public DateTime? AchievedAt { get; set; }
}
public class GoalContributionHistoryDto
{
    public int Id { get; set; }
    public int GoalId { get; set; }
    public decimal Amount { get; set; }
    public ContributionType Type { get; set; }
    public string? Description { get; set; }
    public int? TransactionId { get; set; }
    public DateTime ContributedAt { get; set; }
    public string? TransactionDescription { get; set; }
    public string? AccountName { get; set; }
    public string? AccountType { get; set; }
}
public class GoalMilestoneDto
{
    public int Id { get; set; }
    public int GoalId { get; set; }
    public int Percentage { get; set; }
    public bool IsAchieved { get; set; }
    public DateTime? AchievedAt { get; set; }
    public bool NotificationSent { get; set; }
    public string? GoalName { get; set; }
    public GoalType? GoalType { get; set; }
}
public enum ContributionType
{
    Manual = 0,
    Automatic = 1,
    Transaction = 2,
    Completion = 3
}