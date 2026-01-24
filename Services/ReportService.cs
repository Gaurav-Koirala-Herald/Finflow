
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using FinFlowAPI.DTOs;
using FinFlowAPI.Models;
using FinFlowAPI.DTO;

namespace FinFlowAPI.Services
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<MonthlyReportDto> GenerateMonthlyReportAsync(int userId, DateTime startDate, DateTime endDate)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetMonthlyReport",
                new { UserId = userId, StartDate = startDate, EndDate = endDate },
                commandType: CommandType.StoredProcedure);

            // Main report data
            var mainData = await multi.ReadSingleOrDefaultAsync<dynamic>();
            
            // Expenses by category
            var expensesByCategory = (await multi.ReadAsync<string>()).ToList();
            
            // Income by category
            var incomeByCategory = (await multi.ReadAsync<string>()).ToList();
            
            // Goal progress
            var goalProgress = (await multi.ReadAsync<GoalProgressSummaryDto>()).ToList();

            return new MonthlyReportDto
            {
                Month = startDate,
                TotalIncome = mainData?.TotalIncome ?? 0,
                TotalExpenses = mainData?.TotalExpenses ?? 0,
                NetAmount = mainData?.NetAmount ?? 0,
                TransactionCount = mainData?.TransactionCount ?? 0,
                AverageTransactionAmount = mainData?.AverageTransactionAmount ?? 0,
                ExpensesByCategory = expensesByCategory,
                IncomeByCategory = incomeByCategory,
                GoalProgress = goalProgress
            };
        }

        public async Task<YearlyReportDto> GenerateYearlyReportAsync(int userId, int year)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetYearlyReport",
                new { UserId = userId, Year = year },
                commandType: CommandType.StoredProcedure);

            // Main yearly data
            var mainData = await multi.ReadSingleOrDefaultAsync<dynamic>();
            
            // Monthly breakdown
            var monthlyBreakdown = (await multi.ReadAsync<MonthlyBreakdownDto>()).ToList();
            
            // Top expense categories
            var topExpenseCategories = (await multi.ReadAsync<string>()).ToList();
            
            // Top income categories
            var topIncomeCategories = (await multi.ReadAsync<string>()).ToList();
            
            // Goal achievements
            var goalAchievements = (await multi.ReadAsync<GoalAchievementDto>()).ToList();
            
            // Investment performance
            var investmentPerformance = await multi.ReadSingleOrDefaultAsync<InvestmentPerformanceDto>();

            return new YearlyReportDto
            {
                Year = year,
                TotalIncome = mainData?.TotalIncome ?? 0,
                TotalExpenses = mainData?.TotalExpenses ?? 0,
                NetAmount = mainData?.NetAmount ?? 0,
                TotalTransactions = mainData?.TotalTransactions ?? 0,
                AverageMonthlyIncome = mainData?.AverageMonthlyIncome ?? 0,
                AverageMonthlyExpenses = mainData?.AverageMonthlyExpenses ?? 0,
                MonthlyBreakdown = monthlyBreakdown,
                TopExpenseCategories = topExpenseCategories,
                TopIncomeCategories = topIncomeCategories,
                GoalAchievements = goalAchievements,
                InvestmentPerformance = investmentPerformance
            };
        }

        public async Task<ComparativeAnalysisDto> GenerateComparativeAnalysisAsync(
            int userId, 
            DateTime period1Start, 
            DateTime period1End, 
            DateTime period2Start, 
            DateTime period2End)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetComparativeAnalysis",
                new 
                { 
                    UserId = userId, 
                    Period1Start = period1Start, 
                    Period1End = period1End,
                    Period2Start = period2Start,
                    Period2End = period2End
                },
                commandType: CommandType.StoredProcedure);

            // Income comparison
            var incomeComparison = await multi.ReadSingleOrDefaultAsync<dynamic>();
            
            // Expense comparison
            var expenseComparison = await multi.ReadSingleOrDefaultAsync<dynamic>();
            
            // Net amount comparison
            var netAmountComparison = await multi.ReadSingleOrDefaultAsync<dynamic>();
            
            // Category comparisons
            var categoryComparisons = (await multi.ReadAsync<dynamic>()).ToList();
            
            // Goal comparisons
            var goalComparisons = (await multi.ReadAsync<dynamic>()).ToList();

            return new ComparativeAnalysisDto
            {
                Period1Start = period1Start,
                Period1End = period1End,
                Period2Start = period2Start,
                Period2End = period2End,
                IncomeComparison = CreatePeriodComparison(incomeComparison),
                ExpenseComparison = CreatePeriodComparison(expenseComparison),
                NetAmountComparison = CreatePeriodComparison(netAmountComparison),
                CategoryComparisons = categoryComparisons.Select(CreateCategoryComparison).ToList(),
                GoalComparisons = goalComparisons.Select(CreateGoalComparison).ToList()
            };
        }

        // public async Task<List<TransactionDto>> GetTransactionsForExportAsync(
        //     int userId,
        //     DateTime? startDate = null,
        //     DateTime? endDate = null,
        //     List<int>? categoryIds = null,
        //     List<TransactionType>? transactionTypes = null)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     await connection.OpenAsync();

        //     var categoryIdsString = categoryIds != null && categoryIds.Any() 
        //         ? string.Join(",", categoryIds) 
        //         : null;
            
        //     var transactionTypesString = transactionTypes != null && transactionTypes.Any() 
        //         ? string.Join(",", transactionTypes.Cast<int>()) 
        //         : null;

        //     var transactions = await connection.QueryAsync<dynamic>(
        //         "sp_ExportTransactions",
        //         new 
        //         { 
        //             UserId = userId,
        //             StartDate = startDate,
        //             EndDate = endDate,
        //             CategoryIds = categoryIdsString,
        //             TransactionTypes = transactionTypesString
        //         },
        //         commandType: CommandType.StoredProcedure);

        //     return transactions.Select(t => new TransactionDto
        //     {
        //         Id = t.Id,
        //         Amount = t.Amount,
        //         Type = t.Type == "Income" ? TransactionType.Income : TransactionType.Expense,
        //         CategoryName = t.CategoryName,
        //         Description = t.Description ?? string.Empty,
        //         Date = t.Date,
        //         CreatedAt = t.CreatedAt
        //     }).ToList();
        // }

        // public async Task<List<GoalDto>> GetGoalsForExportAsync(
        //     int userId,
        //     List<GoalType>? goalTypes = null)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     await connection.OpenAsync();

        //     var goalTypesString = goalTypes != null && goalTypes.Any() 
        //         ? string.Join(",", goalTypes.Cast<int>()) 
        //         : null;

        //     var goals = await connection.QueryAsync<dynamic>(
        //         "sp_ExportGoals",
        //         new 
        //         { 
        //             UserId = userId,
        //             GoalTypes = goalTypesString
        //         },
        //         commandType: CommandType.StoredProcedure);

        //     return goals.Select(g => new GoalDto
        //     {
        //         Id = g.Id,
        //         Name = g.Name,
        //         Type = ParseGoalType(g.Type),
        //         TargetAmount = g.TargetAmount,
        //         CurrentAmount = g.CurrentAmount,
        //         ProgressPercentage = g.ProgressPercentage,
        //         Status = ParseGoalStatus(g.Status),
        //         Deadline = g.Deadline,
        //         CreatedAt = g.CreatedAt,
        //         DaysRemaining = g.Deadline > DateTime.UtcNow 
        //             ? (int)(g.Deadline - DateTime.UtcNow).TotalDays 
        //             : 0
        //     }).ToList();
        // }

        // public async Task<InvestmentPerformanceDto> GetInvestmentPerformanceAsync(int userId, DateTime startDate, DateTime endDate)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     await connection.OpenAsync();

        //     // Get investment goals and their performance
        //     var investmentGoals = await connection.QueryAsync<dynamic>(
        //         @"SELECT g.Id as GoalId, g.Name, g.TargetAmount, g.CurrentAmount, g.Status,
        //                  CASE WHEN g.TargetAmount > 0 THEN (g.CurrentAmount / g.TargetAmount) * 100 ELSE 0 END as ProgressPercentage
        //           FROM Goals g 
        //           WHERE g.UserId = @UserId AND g.Type = 1 AND g.CreatedAt >= @StartDate AND g.CreatedAt <= @EndDate",
        //         new { UserId = userId, StartDate = startDate, EndDate = endDate });

        //     var totalInvestmentAmount = investmentGoals.Sum(g => (decimal)g.CurrentAmount);
        //     var activeGoals = investmentGoals.Count(g => (int)g.Status == 0);
        //     var completedGoals = investmentGoals.Count(g => (int)g.Status == 1);

        //     return new InvestmentPerformanceDto
        //     {
        //         TotalInvestmentAmount = totalInvestmentAmount,
        //         CurrentPortfolioValue = totalInvestmentAmount, // Placeholder - would need actual market values
        //         TotalReturn = 0, // Placeholder - would need actual return calculation
        //         ReturnPercentage = 0, // Placeholder - would need actual return percentage
        //         ActiveInvestmentGoals = activeGoals,
        //         CompletedInvestmentGoals = completedGoals,
        //         InvestmentGoals = investmentGoals.Select(g => new InvestmentGoalDto
        //         {
        //             GoalId = g.GoalId,
        //             Name = g.Name,
        //             TargetAmount = g.TargetAmount,
        //             CurrentAmount = g.CurrentAmount,
        //             ProgressPercentage = g.ProgressPercentage,
        //             Status = (GoalStatus)g.Status
        //         }).ToList()
        //     };
        // }

        private static PeriodComparisonDto CreatePeriodComparison(dynamic data)
        {
            if (data == null) return new PeriodComparisonDto();

            var period1Amount = (decimal)(data.Period1Amount ?? 0);
            var period2Amount = (decimal)(data.Period2Amount ?? 0);
            var difference = period2Amount - period1Amount;
            var percentageChange = period1Amount != 0 ? (difference / period1Amount) * 100 : 0;

            return new PeriodComparisonDto
            {
                Period1Amount = period1Amount,
                Period2Amount = period2Amount,
                Difference = difference,
                PercentageChange = percentageChange,
                Trend = difference > 0 ? ComparisonTrend.Increasing : 
                        difference < 0 ? ComparisonTrend.Decreasing : 
                        ComparisonTrend.Stable
            };
        }

        private static CategoryComparisonDto CreateCategoryComparison(dynamic data)
        {
            return new CategoryComparisonDto
            {
                CategoryId = data.CategoryId,
                CategoryName = data.CategoryName ?? string.Empty,
                Type = (TransactionType)data.Type,
                Comparison = CreatePeriodComparison(data)
            };
        }

        private static GoalComparisonDto CreateGoalComparison(dynamic data)
        {
            return new GoalComparisonDto
            {
                GoalId = data.GoalId,
                Name = data.Name ?? string.Empty,
                Type = data.Type,
                Period1Progress = data.Period1Progress ?? 0,
                Period2Progress = data.Period2Progress ?? 0,
                ProgressDifference = (data.Period2Progress ?? 0) - (data.Period1Progress ?? 0)
            };
        }

        // private static GoalType ParseGoalType(string type)
        // {
        //     return type switch
        //     {
        //         "Savings" => GoalType.Savings,
        //         "Investment" => GoalType.Investment,
        //         "DebtRepayment" => GoalType.DebtRepayment,
        //         _ => GoalType.Savings
        //     };
        // }

        // private static GoalStatus ParseGoalStatus(string status)
        // {
        //     return status switch
        //     {
        //         "Active" => GoalStatus.Active,
        //         "Completed" => GoalStatus.Completed,
        //         "Paused" => GoalStatus.Paused,
        //         _ => GoalStatus.Active
        //     };
        // }
    }
}