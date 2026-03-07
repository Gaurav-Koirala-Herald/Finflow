using Dapper;
using FinFlowAPI.Services;

public class WatchlistService : IWatchListService
    {
        private readonly SqlHandlerService _handler;

        public WatchlistService(SqlHandlerService handler)
        {
            _handler = handler;
        }

        public async Task<List<WatchlistItemDTO>> GetAsync(string userId)
        {
            string sp = "sp_GetWatchlist";

            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            var dbResp = await _handler.ExecuteAsyncList<WatchlistItemDTO>(sp, param);
            return dbResp;
        }

        public async Task<CommonResponseDTO> AddAsync(AddWatchlistDTO dto, string userId)
        {
            string sp = "sp_AddToWatchlist";

            var param = new DynamicParameters();
            param.Add("@UserId",     userId);
            param.Add("@Symbol",     dto.Symbol);
            param.Add("@AlertPrice", dto.AlertPrice);

            var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
            return dbResp;
        }

        public async Task<CommonResponseDTO> RemoveAsync(string userId, string symbol)
        {
            string sp = "sp_RemoveFromWatchlist";

            var param = new DynamicParameters();
            param.Add("@UserId", userId);
            param.Add("@Symbol", symbol);

            var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
            return dbResp;
        }
    }
