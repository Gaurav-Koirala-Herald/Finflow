using System.ComponentModel.DataAnnotations;
using FinFlowAPI.Models;
using Models;

namespace FinFlowAPI.DTOs
{
    public class ReportRequestDto
    {
        [Required]
        public ReportType Type { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public List<int>? CategoryIds { get; set; }
        public List<TransactionType>? TransactionTypes { get; set; }
        public List<GoalType>? GoalTypes { get; set; }
        public ReportFormat Format { get; set; } = ReportFormat.Json;
    }

    public class MonthlyReportDto
    {
        public DateTime Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public List<string> ExpensesByCategory { get; set; } = new();
        public List<string> IncomeByCategory { get; set; } = new();
        public List<GoalProgressSummaryDto> GoalProgress { get; set; } = new();
        public int TransactionCount { get; set; }
        public decimal AverageTransactionAmount { get; set; }
    }

    public class YearlyReportDto
    {
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetAmount { get; set; }
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
        public List<string> TopExpenseCategories { get; set; } = new();
        public List<string> TopIncomeCategories { get; set; } = new();
        public List<GoalAchievementDto> GoalAchievements { get; set; } = new();
        public InvestmentPerformanceDto? InvestmentPerformance { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageMonthlyIncome { get; set; }
        public decimal AverageMonthlyExpenses { get; set; }
    }

    public class MonthlyBreakdownDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetAmount { get; set; }
        public int TransactionCount { get; set; }
    }

    public class GoalProgressSummaryDto
    {
        public int GoalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public GoalType Type { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public GoalStatus Status { get; set; }
        public DateTime Deadline { get; set; }
        public int DaysRemaining { get; set; }
        public decimal ContributionsInPeriod { get; set; }
    }

    public class GoalAchievementDto
    {
        public int GoalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public GoalType Type { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime CompletedAt { get; set; }
        public int DaysToComplete { get; set; }
        public decimal TotalContributions { get; set; }
    }

    public class InvestmentPerformanceDto
    {
        public decimal TotalInvestmentAmount { get; set; }
        public decimal CurrentPortfolioValue { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal ReturnPercentage { get; set; }
        public List<InvestmentGoalDto> InvestmentGoals { get; set; } = new();
        public int ActiveInvestmentGoals { get; set; }
        public int CompletedInvestmentGoals { get; set; }
    }

    public class InvestmentGoalDto
    {
        public int GoalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public GoalStatus Status { get; set; }
    }

    public class ComparativeAnalysisDto
    {
        public DateTime Period1Start { get; set; }
        public DateTime Period1End { get; set; }
        public DateTime Period2Start { get; set; }
        public DateTime Period2End { get; set; }
        
        public PeriodComparisonDto IncomeComparison { get; set; } = new();
        public PeriodComparisonDto ExpenseComparison { get; set; } = new();
        public PeriodComparisonDto NetAmountComparison { get; set; } = new();
        
        public List<CategoryComparisonDto> CategoryComparisons { get; set; } = new();
        public List<GoalComparisonDto> GoalComparisons { get; set; } = new();
    }

    public class PeriodComparisonDto
    {
        public decimal Period1Amount { get; set; }
        public decimal Period2Amount { get; set; }
        public decimal Difference { get; set; }
        public decimal PercentageChange { get; set; }
        public ComparisonTrend Trend { get; set; }
    }

    public class CategoryComparisonDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public PeriodComparisonDto Comparison { get; set; } = new();
    }

    public class GoalComparisonDto
    {
        public int GoalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; }
        public decimal Period1Progress { get; set; }
        public decimal Period2Progress { get; set; }
        public decimal ProgressDifference { get; set; }
    }

    public class ExportRequestDto
    {
        [Required]
        public ExportType Type { get; set; }
        
        [Required]
        public ExportFormat Format { get; set; }
        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<TransactionType>? TransactionTypes { get; set; }
        public List<GoalType>? GoalTypes { get; set; }
        public bool IncludeGoals { get; set; } = true;
        public bool IncludeCategories { get; set; } = true;
    }

    public class ExportResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();
        public long FileSize { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public enum  ReportType
    {
        Monthly,
        Yearly,
        Custom,
        Comparative
    }

    public enum ReportFormat
    {
        Json,
        Pdf,
        Excel,
        Csv
    }

    public enum ExportType
    {
        Transactions,
        Goals,
        Complete,
        Summary
    }

    public enum ExportFormat
    {
        Excel,
        Csv,
        Json
    }

    public enum ComparisonTrend
    {
        Increasing,
        Decreasing,
        Stable
    }
}