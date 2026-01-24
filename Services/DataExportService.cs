using System.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;
using System.Text;
using ClosedXML.Excel;
using System.Globalization;
using FinFlowAPI.DTO.Goals;
using FinFlowAPI.DTOs;

namespace FinFlowAPI.Services
{
    public class DataExportService
    {
        private readonly string _connectionString;
        private readonly ReportService _reportService;

        public DataExportService(IConfiguration configuration, ReportService reportService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
            _reportService = reportService;
        }

        // public async Task<ExportResultDto> ExportTransactionsAsync(int userId, ExportRequestDto request)
        // {
        //     var transactions = await _reportService.GetTransactionsForExportAsync(
        //         userId,
        //         request.StartDate,
        //         request.EndDate,
        //         request.CategoryIds,
        //         request.TransactionTypes);

        //     var goals = request.IncludeGoals 
        //         ? await _reportService.GetGoalsForExportAsync(userId, request.GoalTypes)
        //         : new List<GoalDto>();

        //     return request.Format switch
        //     {
        //         ExportFormat.Excel => await GenerateExcelExportAsync(transactions, goals, request),
        //         ExportFormat.Csv => GenerateCsvExport(transactions, goals, request),
        //         ExportFormat.Json => GenerateJsonExport(transactions, goals, request),
        //         _ => throw new ArgumentException($"Unsupported export format: {request.Format}")
        //     };
        // }

        // public async Task<ExportResultDto> ExportCompleteDataAsync(int userId, ExportRequestDto request)
        // {
        //     var transactions = await _reportService.GetTransactionsForExportAsync(
        //         userId,
        //         request.StartDate,
        //         request.EndDate,
        //         request.CategoryIds,
        //         request.TransactionTypes);

        //     var goals = await _reportService.GetGoalsForExportAsync(userId, request.GoalTypes);

        //     return request.Format switch
        //     {
        //         ExportFormat.Excel => await GenerateCompleteExcelExportAsync(transactions, goals, userId, request),
        //         ExportFormat.Csv => GenerateCompleteCsvExport(transactions, goals, request),
        //         ExportFormat.Json => GenerateCompleteJsonExport(transactions, goals, request),
        //         _ => throw new ArgumentException($"Unsupported export format: {request.Format}")
        //     };
        // }

        private async Task<ExportResultDto> GenerateExcelExportAsync(
            List<TransactionDto> transactions, 
            List<GoalDto> goals, 
            ExportRequestDto request)
        {
            using var workbook = new XLWorkbook();
            
            // Transactions worksheet
            var transactionSheet = workbook.Worksheets.Add("Transactions");
            AddTransactionsToWorksheet(transactionSheet, transactions);
            
            // Goals worksheet (if included)
            if (request.IncludeGoals && goals.Any())
            {
                var goalSheet = workbook.Worksheets.Add("Goals");
                // AddGoalsToWorksheet(goalSheet, goals);
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var data = stream.ToArray();

            var fileName = $"financial_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            return new ExportResultDto
            {
                FileName = fileName,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Data = data,
                FileSize = data.Length,
                GeneratedAt = DateTime.UtcNow
            };
        }

        private async Task<ExportResultDto> GenerateCompleteExcelExportAsync(
            List<TransactionDto> transactions, 
            List<GoalDto> goals, 
            int userId,
            ExportRequestDto request)
        {
            using var workbook = new XLWorkbook();
            
            // Transactions worksheet
            var transactionSheet = workbook.Worksheets.Add("Transactions");
            AddTransactionsToWorksheet(transactionSheet, transactions);
            
            // Goals worksheet
            var goalSheet = workbook.Worksheets.Add("Goals");
            // AddGoalsToWorksheet(goalSheet, goals);

            // Summary worksheet
            var summarySheet = workbook.Worksheets.Add("Summary");
            await AddSummaryToWorksheetAsync(summarySheet, userId, request);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var data = stream.ToArray();

            var fileName = $"complete_financial_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            return new ExportResultDto
            {
                FileName = fileName,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Data = data,
                FileSize = data.Length,
                GeneratedAt = DateTime.UtcNow
            };
        }

        private ExportResultDto GenerateCsvExport(
            List<TransactionDto> transactions, 
            List<GoalDto> goals, 
            ExportRequestDto request)
        {
            var csv = new StringBuilder();
            
            // Transactions section
            csv.AppendLine("TRANSACTIONS");
            csv.AppendLine("Date,Type,Category,Amount,Description");
            
            foreach (var transaction in transactions)
            {
                csv.AppendLine($"{transaction.TransactionDate:yyyy-MM-dd}," +
                              $"{(transaction.transactionTypeId == 1 ? "Income" : "Expense") }," +
                              $"\"{transaction.categoryId}\"," +
                              $"{transaction.Amount:F2}," +
                              $"\"{EscapeCsvValue(transaction.Description)}\"");
            }

            // Goals section (if included)
            if (request.IncludeGoals && goals.Any())
            {
                csv.AppendLine();
                csv.AppendLine("GOALS");
                csv.AppendLine("Name,Type,Target Amount,Current Amount,Progress %,Status,Deadline");
                
                foreach (var goal in goals)
                {
                    csv.AppendLine($"\"{EscapeCsvValue(goal.Name)}\"," +
                                  $"{goal.Type}," +
                                  $"{goal.TargetAmount:F2}," +
                                  $"{goal.CurrentAmount:F2}," +
                                  $"{goal.ProgressPercentage:F1}," +
                                  $"{goal.Status}," +
                                  $"{goal.Deadline:yyyy-MM-dd}");
                }
            }

            var data = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"financial_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            
            return new ExportResultDto
            {
                FileName = fileName,
                ContentType = "text/csv",
                Data = data,
                FileSize = data.Length,
                GeneratedAt = DateTime.UtcNow
            };
        }

        // private ExportResultDto GenerateCompleteCsvExport(
        //     List<TransactionDto> transactions, 
        //     List<GoalDto> goals, 
        //     ExportRequestDto request)
        // {
        //     var csv = new StringBuilder();
            
        //     // Summary section
        //     csv.AppendLine("FINANCIAL SUMMARY");
        //     csv.AppendLine($"Export Date,{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        //     csv.AppendLine($"Total Transactions,{transactions.Count}");
        //     csv.AppendLine($"Total Goals,{goals.Count}");
        //     csv.AppendLine($"Total Income,{transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount):F2}");
        //     csv.AppendLine($"Total Expenses,{transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount):F2}");
        //     csv.AppendLine();
            
        //     // Transactions section
        //     csv.AppendLine("TRANSACTIONS");
        //     csv.AppendLine("Date,Type,Category,Amount,Description");
            
        //     foreach (var transaction in transactions)
        //     {
        //         csv.AppendLine($"{transaction.Date:yyyy-MM-dd}," +
        //                       $"{transaction.Type}," +
        //                       $"\"{transaction.CategoryName}\"," +
        //                       $"{transaction.Amount:F2}," +
        //                       $"\"{EscapeCsvValue(transaction.Description)}\"");
        //     }

        //     // Goals section
        //     csv.AppendLine();
        //     csv.AppendLine("GOALS");
        //     csv.AppendLine("Name,Type,Target Amount,Current Amount,Progress %,Status,Deadline,Days Remaining");
            
        //     foreach (var goal in goals)
        //     {
        //         csv.AppendLine($"\"{EscapeCsvValue(goal.Name)}\"," +
        //                       $"{goal.Type}," +
        //                       $"{goal.TargetAmount:F2}," +
        //                       $"{goal.CurrentAmount:F2}," +
        //                       $"{goal.ProgressPercentage:F1}," +
        //                       $"{goal.Status}," +
        //                       $"{goal.Deadline:yyyy-MM-dd}," +
        //                       $"{goal.DaysRemaining}");
        //     }

        //     var data = Encoding.UTF8.GetBytes(csv.ToString());
        //     var fileName = $"complete_financial_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            
        //     return new ExportResultDto
        //     {
        //         FileName = fileName,
        //         ContentType = "text/csv",
        //         Data = data,
        //         FileSize = data.Length,
        //         GeneratedAt = DateTime.UtcNow
        //     };
        // }

        private ExportResultDto GenerateJsonExport(
            List<TransactionDto> transactions, 
            List<GoalDto> goals, 
            ExportRequestDto request)
        {
            var exportData = new
            {
                ExportInfo = new
                {
                    GeneratedAt = DateTime.UtcNow,
                    Type = request.Type.ToString(),
                    Format = request.Format.ToString(),
                    Filters = new
                    {
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        CategoryIds = request.CategoryIds,
                        TransactionTypes = request.TransactionTypes,
                        GoalTypes = request.GoalTypes
                    }
                },
                Transactions = transactions,
                Goals = request.IncludeGoals ? goals : null
            };

            var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            var data = Encoding.UTF8.GetBytes(json);
            var fileName = $"financial_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            
            return new ExportResultDto
            {
                FileName = fileName,
                ContentType = "application/json",
                Data = data,
                FileSize = data.Length,
                GeneratedAt = DateTime.UtcNow
            };
        }

        // private ExportResultDto GenerateCompleteJsonExport(
        //     List<TransactionDto> transactions, 
        //     List<GoalDto> goals, 
        //     ExportRequestDto request)
        // {
        //     var summary = new
        //     {
        //         TotalTransactions = transactions.Count,
        //         TotalGoals = goals.Count,
        //         TotalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
        //         TotalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
        //         ActiveGoals = goals.Count(g => g.Status == GoalStatus.Active),
        //         CompletedGoals = goals.Count(g => g.Status == GoalStatus.Completed)
        //     };

        //     var exportData = new
        //     {
        //         ExportInfo = new
        //         {
        //             GeneratedAt = DateTime.UtcNow,
        //             Type = "Complete",
        //             Format = request.Format.ToString()
        //         },
        //         Summary = summary,
        //         Transactions = transactions,
        //         Goals = goals
        //     };

        //     var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
        //     {
        //         WriteIndented = true,
        //         PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        //     });

        //     var data = Encoding.UTF8.GetBytes(json);
        //     var fileName = $"complete_financial_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            
        //     return new ExportResultDto
        //     {
        //         FileName = fileName,
        //         ContentType = "application/json",
        //         Data = data,
        //         FileSize = data.Length,
        //         GeneratedAt = DateTime.UtcNow
        //     };
        // }

        private void AddTransactionsToWorksheet(IXLWorksheet worksheet, List<TransactionDto> transactions)
        {
            // Headers
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Type";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "Amount (NPR)";
            worksheet.Cell(1, 5).Value = "Description";
            
            // Format headers
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
            // Data
            for (int i = 0; i < transactions.Count; i++)
            {
                var transaction = transactions[i];
                var row = i + 2;
                
                worksheet.Cell(row, 1).Value = transaction.TransactionDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 2).Value = transaction.transactionTypeId == 1 ? "Income" : "Expense";
                worksheet.Cell(row, 3).Value = transaction.Amount;
                worksheet.Cell(row, 4).Value = transaction.Description;
                
                // Format amount column
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                
                // Color code by type
                if (transaction.transactionTypeId == 1)
                {
                    worksheet.Cell(row, 2).Style.Font.FontColor = XLColor.Green;
                }
                else
                {
                    worksheet.Cell(row, 2).Style.Font.FontColor = XLColor.Red;
                }
            }
            
            // Auto-fit columns
            worksheet.ColumnsUsed().AdjustToContents();
        }

        // private void AddGoalsToWorksheet(IXLWorksheet worksheet, List<GoalDto> goals)
        // {
        //     // Headers
        //     worksheet.Cell(1, 1).Value = "Name";
        //     worksheet.Cell(1, 2).Value = "Type";
        //     worksheet.Cell(1, 3).Value = "Target Amount (NPR)";
        //     worksheet.Cell(1, 4).Value = "Current Amount (NPR)";
        //     worksheet.Cell(1, 5).Value = "Progress %";
        //     worksheet.Cell(1, 6).Value = "Status";
        //     worksheet.Cell(1, 7).Value = "Deadline";
        //     worksheet.Cell(1, 8).Value = "Days Remaining";
            
        //     // Format headers
        //     var headerRange = worksheet.Range(1, 1, 1, 8);
        //     headerRange.Style.Font.Bold = true;
        //     headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            
        //     // Data
        //     for (int i = 0; i < goals.Count; i++)
        //     {
        //         var goal = goals[i];
        //         var row = i + 2;
                
        //         worksheet.Cell(row, 1).Value = goal.Name;
        //         worksheet.Cell(row, 2).Value = goal.Type.ToString();
        //         worksheet.Cell(row, 3).Value = goal.TargetAmount;
        //         worksheet.Cell(row, 4).Value = goal.CurrentAmount;
        //         worksheet.Cell(row, 5).Value = goal.ProgressPercentage;
        //         worksheet.Cell(row, 6).Value = goal.Status.ToString();
        //         worksheet.Cell(row, 7).Value = goal.Deadline.ToString("yyyy-MM-dd");
        //         worksheet.Cell(row, 8).Value = goal.DaysRemaining;
                
        //         // Format amount columns
        //         worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
        //         worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
        //         worksheet.Cell(row, 5).Style.NumberFormat.Format = "0.0%";
                
        //         // Color code by status
        //         switch (goal.Status)
        //         {
        //             case GoalStatus.Completed:
        //                 worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Green;
        //                 break;
        //             case GoalStatus.Paused:
        //                 worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Orange;
        //                 break;
        //             default:
        //                 worksheet.Cell(row, 6).Style.Font.FontColor = XLColor.Blue;
        //                 break;
        //         }
        //     }
            
        //     // Auto-fit columns
        //     worksheet.ColumnsUsed().AdjustToContents();
        // }

        private async Task AddSummaryToWorksheetAsync(IXLWorksheet worksheet, int userId, ExportRequestDto request)
        {
            // Title
            worksheet.Cell(1, 1).Value = "Financial Summary Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            // Export info
            worksheet.Cell(3, 1).Value = "Export Date:";
            worksheet.Cell(3, 2).Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
            
            worksheet.Cell(4, 1).Value = "Period:";
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                worksheet.Cell(4, 2).Value = $"{request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd}";
            }
            else
            {
                worksheet.Cell(4, 2).Value = "All Time";
            }

            // Get summary data
            var startDate = request.StartDate ?? DateTime.MinValue;
            var endDate = request.EndDate ?? DateTime.MaxValue;
            
            try
            {
                var monthlyReport = await _reportService.GenerateMonthlyReportAsync(userId, startDate, endDate);
                
                worksheet.Cell(6, 1).Value = "Financial Overview:";
                worksheet.Cell(6, 1).Style.Font.Bold = true;
                
                worksheet.Cell(7, 1).Value = "Total Income:";
                worksheet.Cell(7, 2).Value = monthlyReport.TotalIncome;
                worksheet.Cell(7, 2).Style.NumberFormat.Format = "#,##0.00";
                
                worksheet.Cell(8, 1).Value = "Total Expenses:";
                worksheet.Cell(8, 2).Value = monthlyReport.TotalExpenses;
                worksheet.Cell(8, 2).Style.NumberFormat.Format = "#,##0.00";
                
                worksheet.Cell(9, 1).Value = "Net Amount:";
                worksheet.Cell(9, 2).Value = monthlyReport.NetAmount;
                worksheet.Cell(9, 2).Style.NumberFormat.Format = "#,##0.00";
                
                worksheet.Cell(10, 1).Value = "Total Transactions:";
                worksheet.Cell(10, 2).Value = monthlyReport.TransactionCount;
                
                worksheet.Cell(11, 1).Value = "Average Transaction:";
                worksheet.Cell(11, 2).Value = monthlyReport.AverageTransactionAmount;
                worksheet.Cell(11, 2).Style.NumberFormat.Format = "#,##0.00";
            }
            catch
            {
                worksheet.Cell(6, 1).Value = "Summary data unavailable";
            }
            
            // Auto-fit columns
            worksheet.ColumnsUsed().AdjustToContents();
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
                
            return value.Replace("\"", "\"\"");
        }
    }
}