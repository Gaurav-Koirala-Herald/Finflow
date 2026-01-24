using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FinFlowAPI.Services;
using FinFlowAPI.Models;
using FinFlowAPI.DTOs;
using Models;

namespace FinFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly DataExportService _dataExportService;

        public ReportsController(ReportService reportService, DataExportService dataExportService)
        {
            _reportService = reportService;
            _dataExportService = dataExportService;
        }

        /// <summary>
        /// Generate a monthly financial report
        /// </summary>
        [HttpGet("monthly")]
        public async Task<ActionResult<MonthlyReportDto>> GetMonthlyReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = GetCurrentUserId();
                var report = await _reportService.GenerateMonthlyReportAsync(userId, startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to generate monthly report", error = ex.Message });
            }
        }

        /// <summary>
        /// Generate a yearly financial report
        /// </summary>
        [HttpGet("yearly")]
        public async Task<ActionResult<YearlyReportDto>> GetYearlyReport([FromQuery] int year)
        {
            try
            {
                var userId = GetCurrentUserId();
                var report = await _reportService.GenerateYearlyReportAsync(userId, year);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to generate yearly report", error = ex.Message });
            }
        }

        /// <summary>
        /// Generate a comparative analysis between two time periods
        /// </summary>
        [HttpGet("comparative")]
        public async Task<ActionResult<ComparativeAnalysisDto>> GetComparativeAnalysis(
            [FromQuery] DateTime period1Start,
            [FromQuery] DateTime period1End,
            [FromQuery] DateTime period2Start,
            [FromQuery] DateTime period2End)
        {
            try
            {
                var userId = GetCurrentUserId();
                var analysis = await _reportService.GenerateComparativeAnalysisAsync(
                    userId, period1Start, period1End, period2Start, period2End);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to generate comparative analysis", error = ex.Message });
            }
        }

        /// <summary>
        /// Get investment performance report
        /// </summary>
        // [HttpGet("investment-performance")]
        // public async Task<ActionResult<InvestmentPerformanceDto>> GetInvestmentPerformance(
        //     [FromQuery] DateTime startDate,
        //     [FromQuery] DateTime endDate)
        // {
        //     try
        //     {
        //         var userId = GetCurrentUserId();
        //         var performance = await _reportService.GetInvestmentPerformanceAsync(userId, startDate, endDate);
        //         return Ok(performance);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = "Failed to get investment performance", error = ex.Message });
        //     }
        // }

        /// <summary>
        /// Export transaction data in specified format
        /// </summary>
        // [HttpPost("export/transactions")]
        // public async Task<IActionResult> ExportTransactions([FromBody] ExportRequestDto request)
        // {
        //     try
        //     {
        //         var userId = GetCurrentUserId();
        //         request.Type = ExportType.Transactions;
                
        //         var result = await _dataExportService.ExportTransactionsAsync(userId, request);
                
        //         return File(result.Data, result.ContentType, result.FileName);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = "Failed to export transactions", error = ex.Message });
        //     }
        // }

        /// <summary>
        /// Export complete financial data (transactions + goals + summary)
        /// </summary>
        // [HttpPost("export/complete")]
        // public async Task<IActionResult> ExportCompleteData([FromBody] ExportRequestDto request)
        // {
        //     try
        //     {
        //         var userId = GetCurrentUserId();
        //         request.Type = ExportType.Complete;
                
        //         var result = await _dataExportService.ExportCompleteDataAsync(userId, request);
                
        //         return File(result.Data, result.ContentType, result.FileName);
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = "Failed to export complete data", error = ex.Message });
        //     }
        // }

        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GeneratePdfReport([FromBody] ReportRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                

                object reportData = request.Type switch
                {
                    ReportType.Monthly => await _reportService.GenerateMonthlyReportAsync(userId, request.StartDate, request.EndDate),
                    ReportType.Yearly => await _reportService.GenerateYearlyReportAsync(userId, request.StartDate.Year),
                    ReportType.Comparative => await _reportService.GenerateComparativeAnalysisAsync(
                        userId, request.StartDate, request.EndDate, 
                        request.StartDate.AddYears(-1), request.EndDate.AddYears(-1)),
                    _ => throw new ArgumentException($"Unsupported report type: {request.Type}")
                };

                var json = System.Text.Json.JsonSerializer.Serialize(reportData, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });

                var data = System.Text.Encoding.UTF8.GetBytes(json);
                var fileName = $"{request.Type.ToString().ToLower()}_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
                
                return File(data, "application/json", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to generate PDF report", error = ex.Message });
            }
        }

        /// <summary>
        /// Get available export formats and options
        /// </summary>
        [HttpGet("export/options")]
        public ActionResult<object> GetExportOptions()
        {
            var options = new
            {
                ExportTypes = GetEnumOptions<ExportType>(),
                ExportFormats = GetEnumOptions<ExportFormat>(),
                ReportTypes = GetEnumOptions<ReportType>(),
                GoalTypes = GetEnumOptions<GoalType>()
            };

            return Ok(options);
        }

        /// <summary>
        /// Get report generation status and history
        /// </summary>
        [HttpGet("history")]
        public ActionResult<object> GetReportHistory()
        {
            var history = new
            {
                RecentReports = new[]
                {
                    new { Type = "Monthly", GeneratedAt = DateTime.UtcNow.AddDays(-1), Status = "Completed" },
                    new { Type = "Yearly", GeneratedAt = DateTime.UtcNow.AddDays(-7), Status = "Completed" },
                    new { Type = "Export", GeneratedAt = DateTime.UtcNow.AddDays(-14), Status = "Completed" }
                },
                TotalReportsGenerated = 15,
                LastReportDate = DateTime.UtcNow.AddDays(-1)
            };

            return Ok(history);
        }

        private IEnumerable<object> GetEnumOptions<T>() where T : System.Enum
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>().Select(e => new { Value = (int)(object)e, Name = e.ToString() });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }
}