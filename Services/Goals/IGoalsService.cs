using FinFlowAPI.DTO.Goals;

public interface IGoalService
    {
        Task<IEnumerable<GoalDto>> GetUserGoalsAsync(int userId);
        Task<GoalDto?> GetGoalByIdAsync(int goalId, int userId);
        Task<GoalDto> CreateGoalAsync(CreateGoalDto createGoalDto, int userId);
        Task<GoalDto?> UpdateGoalAsync(int goalId, UpdateGoalDto updateGoalDto, int userId);
        Task<bool> DeleteGoalAsync(int goalId, int userId);
        Task<GoalProgressDto?> GetGoalProgressAsync(int goalId, int userId);
        Task UpdateGoalProgressAsync(int userId);
        Task<IEnumerable<GoalDto>> GetGoalsNearingDeadlineAsync(int userId, int daysThreshold = 30);
        Task<bool> AddGoalContributionAsync(int goalId, int userId, decimal amount, string? description = null, int? transactionId = null);
        Task<bool> CompleteGoalAsync(int goalId, int userId, string? completionNote = null);
        Task<IEnumerable<GoalContributionHistoryDto>> GetGoalContributionHistoryAsync(int goalId, int userId);
        Task<IEnumerable<GoalMilestoneDto>> GetGoalMilestonesAsync(int goalId);
        Task<IEnumerable<GoalMilestoneDto>> GetRecentAchievementsAsync(int userId, int daysBack = 7);
        Task CheckAndUpdateMilestonesAsync(int goalId);
    }