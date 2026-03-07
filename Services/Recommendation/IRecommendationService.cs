 public interface IRecommendationService
    {
        Task<List<RecommendationDTO>> GetCachedAsync(string userId, int maxAgeHours = 6);
        Task<CommonResponseDTO> SaveAsync(string userId, StockCache stock,
            decimal score, ScoreBreakdown breakdown, string aiExplanation);
        Task<CommonResponseDTO> DeleteAllForUserAsync(string userId);
    }