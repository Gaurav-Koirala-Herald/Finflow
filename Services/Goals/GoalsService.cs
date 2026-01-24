using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using FinFlowAPI.DTO.Goals;
using Models;

namespace FinFlowAPI.Services
{


    public class GoalService : IGoalService
    {
        private readonly string _connectionString;

        public GoalService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<GoalDto>> GetUserGoalsAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var goals = await connection.QueryAsync<GoalDto>(
                "sp_GetUserGoals",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);

            return goals;
        }

        public async Task<GoalDto?> GetGoalByIdAsync(int goalId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var goal = await connection.QuerySingleOrDefaultAsync<GoalDto>(
                "sp_GetGoalById",
                new { GoalId = goalId, UserId = userId },
                commandType: CommandType.StoredProcedure);

            return goal;
        }

        public async Task<GoalDto> CreateGoalAsync(CreateGoalDto createGoalDto, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var goal = await connection.QuerySingleAsync<GoalDto>(
                "sp_CreateGoal",
                new 
                { 
                    UserId = userId,
                    Name = createGoalDto.Name,
                    TargetAmount = createGoalDto.TargetAmount,
                    Deadline = createGoalDto.Deadline,
                    Type = (int)createGoalDto.Type
                },
                commandType: CommandType.StoredProcedure);

            return goal;
        }

        public async Task<GoalDto?> UpdateGoalAsync(int goalId, UpdateGoalDto updateGoalDto, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var goal = await connection.QuerySingleOrDefaultAsync<GoalDto>(
                "sp_UpdateGoal",
                new 
                { 
                    GoalId = goalId,
                    UserId = userId,
                    Name = updateGoalDto.Name,
                    TargetAmount = updateGoalDto.TargetAmount,
                    CurrentAmount = updateGoalDto.CurrentAmount,
                    Deadline = updateGoalDto.Deadline,
                    Type = updateGoalDto.Type.HasValue ? (int)updateGoalDto.Type.Value : (int?)null,
                    Status = updateGoalDto.Status.HasValue ? (int)updateGoalDto.Status.Value : (int?)null
                },
                commandType: CommandType.StoredProcedure);

            return goal;
        }

        public async Task<bool> DeleteGoalAsync(int goalId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QuerySingleAsync<dynamic>(
                "sp_DeleteGoal",
                new { GoalId = goalId, UserId = userId },
                commandType: CommandType.StoredProcedure);

            return result.Success == 1;
        }

        public async Task<GoalProgressDto?> GetGoalProgressAsync(int goalId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetGoalProgress",
                new { GoalId = goalId, UserId = userId },
                commandType: CommandType.StoredProcedure);

            // Goal basic info
            var goalInfo = await multi.ReadSingleOrDefaultAsync<dynamic>();
            if (goalInfo == null) return null;

            // Milestones
            var milestones = (await multi.ReadAsync<MilestoneDto>()).ToList();

            return new GoalProgressDto
            {
                GoalId = goalInfo.GoalId,
                Name = goalInfo.Name,
                TargetAmount = goalInfo.TargetAmount,
                CurrentAmount = goalInfo.CurrentAmount,
                ProgressPercentage = goalInfo.ProgressPercentage,
                Status = goalInfo.Status,
                Milestones = milestones
            };
        }

        public async Task UpdateGoalProgressAsync(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "sp_UpdateGoalProgressFromTransactions",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<GoalDto>> GetGoalsNearingDeadlineAsync(int userId, int daysThreshold = 30)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var goals = await connection.QueryAsync<GoalDto>(
                "sp_GetGoalsNearingDeadline",
                new { UserId = userId, DaysThreshold = daysThreshold },
                commandType: CommandType.StoredProcedure);

            return goals;
        }

        public async Task<bool> AddGoalContributionAsync(int goalId, int userId, decimal amount, string? description = null, int? transactionId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QuerySingleAsync<dynamic>(
                "sp_AddGoalContribution",
                new 
                { 
                    GoalId = goalId,
                    UserId = userId,
                    Amount = amount,
                    Description = description,
                    TransactionId = transactionId
                },
                commandType: CommandType.StoredProcedure);

            return result.Success == 1;
        }

        public async Task<bool> CompleteGoalAsync(int goalId, int userId, string? completionNote = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QuerySingleAsync<dynamic>(
                "sp_CompleteGoal",
                new 
                { 
                    GoalId = goalId,
                    UserId = userId,
                    CompletionNote = completionNote
                },
                commandType: CommandType.StoredProcedure);

            return result.Success == 1;
        }

        public async Task<IEnumerable<GoalContributionHistoryDto>> GetGoalContributionHistoryAsync(int goalId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var contributions = await connection.QueryAsync<GoalContributionHistoryDto>(
                "sp_GetGoalContributionHistory",
                new { GoalId = goalId, UserId = userId },
                commandType: CommandType.StoredProcedure);

            return contributions;
        }

        public async Task<IEnumerable<GoalMilestoneDto>> GetGoalMilestonesAsync(int goalId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var milestones = await connection.QueryAsync<GoalMilestoneDto>(
                "sp_GetGoalMilestones",
                new { GoalId = goalId },
                commandType: CommandType.StoredProcedure);

            return milestones;
        }

        public async Task<IEnumerable<GoalMilestoneDto>> GetRecentAchievementsAsync(int userId, int daysBack = 7)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var achievements = await connection.QueryAsync<GoalMilestoneDto>(
                "sp_GetRecentAchievements",
                new { UserId = userId, DaysBack = daysBack },
                commandType: CommandType.StoredProcedure);

            return achievements;
        }

        public async Task CheckAndUpdateMilestonesAsync(int goalId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "sp_CheckAndUpdateMilestones",
                new { GoalId = goalId },
                commandType: CommandType.StoredProcedure);
        }
    }
}