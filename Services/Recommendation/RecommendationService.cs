using Dapper;
using FinFlowAPI.Services;

public class RecommendationService : IRecommendationService
    {
        private readonly SqlHandlerService _handler;

        public RecommendationService(SqlHandlerService handler)
        {
            _handler = handler;
        }

        public async Task<List<RecommendationDTO>> GetCachedAsync(
            string userId, int maxAgeHours = 6)
        {
            string sp = "sp_GetRecommendations";

            var param = new DynamicParameters();
            param.Add("@UserId",      userId);
            param.Add("@MaxAgeHours", maxAgeHours);

            var dbResp = await _handler.ExecuteAsyncList<RecommendationDTO>(sp, param);
            return dbResp;
        }

        public async Task<CommonResponseDTO> SaveAsync(
            string userId, StockCache stock, decimal score,
            ScoreBreakdown breakdown, string aiExplanation)
        {
            string sp = "sp_SaveRecommendation";

            var param = new DynamicParameters();
            param.Add("@UserId",            userId);
            param.Add("@Symbol",            stock.Symbol);
            param.Add("@Score",             score);
            param.Add("@TrendScore",        breakdown.TrendScore);
            param.Add("@FundamentalsScore", breakdown.FundamentalsScore);
            param.Add("@SectorScore",       breakdown.SectorScore);
            param.Add("@RiskScore",         breakdown.RiskScore);
            param.Add("@PopularityScore",   breakdown.PopularityScore);
            param.Add("@AiExplanation",     aiExplanation);

            var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
            return dbResp;
        }

        public async Task<CommonResponseDTO> DeleteAllForUserAsync(string userId)
        {
            string sp = "sp_DeleteOldRecommendations";

            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
            return dbResp;
        }
    }