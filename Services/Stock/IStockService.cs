public interface IStockService
{
    Task<List<StockCache>> GetStocksForScoringAsync(string userId);
    Task<CommonResponseDTO> UpsertStockAsync(StockCache stock);

    Task<int> BulkUpsertAsync(List<StockCache> stocks);
}