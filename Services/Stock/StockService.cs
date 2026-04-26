using Dapper;
using FinFlowAPI.Services;

public class StockService : IStockService
{
    private readonly SqlHandlerService _handler;

    public StockService(SqlHandlerService handler)
    {
        _handler = handler;
    }

    public async Task<List<StockCache>> GetStocksForScoringAsync(string userId)
    {
        string sp = "sp_GetStocksForScoring";

        var param = new DynamicParameters();
        param.Add("@UserId", userId);

        var dbResp = await _handler.ExecuteAsyncList<StockCache>(sp, param);
        return dbResp;
    }

    public async Task<CommonResponseDTO> UpsertStockAsync(StockCache stock)
    {
        string sp = "sp_UpsertStockCache";

        var param = new DynamicParameters();
        param.Add("@Symbol", stock.Symbol);
        param.Add("@SecurityName", stock.SecurityName);
        param.Add("@Sector", stock.Sector);
        param.Add("@CurrentPrice", stock.CurrentPrice);
        param.Add("@Ltp", stock.Ltp);
        param.Add("@PointChange", stock.PointChange);
        param.Add("@PercentageChange", stock.PercentageChange);
        param.Add("@PriceChange30d", stock.PriceChange30d);
        param.Add("@PeRatio", stock.PeRatio);
        param.Add("@MarketCap", stock.MarketCap);
        param.Add("@Volume", stock.Volume);
        param.Add("@High52week", stock.High52week);
        param.Add("@Low52week", stock.Low52week);
        param.Add("@Eps", stock.Eps);
        param.Add("@BookValue", stock.BookValue);

        var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
        return dbResp;
    }
    public async Task<int> BulkUpsertAsync(List<StockCache> stocks)
    {
        var tasks = stocks.Select(s=>UpsertStockAsync(s));
        await Task.WhenAll(tasks);
        return stocks.Count;
    }
}

