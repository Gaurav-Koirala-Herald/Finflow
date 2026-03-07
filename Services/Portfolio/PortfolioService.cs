using Dapper;
using FinFlowAPI.Services;

public class PortfolioService : IPortfolioService
    {
        private readonly SqlHandlerService _handler;

        public PortfolioService(SqlHandlerService handler)
        {
            _handler = handler;
        }

        public async Task<List<PortfolioItemDTO>> GetAsync(string userId)
        {
            string sp = "sp_GetPortfolio";

            var param = new DynamicParameters();
            param.Add("@UserId", userId);

            var dbResp = await _handler.ExecuteAsyncList<PortfolioItemDTO>(sp, param);
            return dbResp;
        }

        public async Task<CommonResponseDTO> AddAsync(AddPortfolioDTO dto, string userId)
        {
            string sp = "sp_AddToPortfolio";

            var param = new DynamicParameters();
            param.Add("@UserId",   userId);
            param.Add("@Symbol",   dto.Symbol);
            param.Add("@Quantity", dto.Quantity);
            param.Add("@BuyPrice", dto.BuyPrice);

            var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
            return dbResp;
        }

        public async Task<CommonResponseDTO> RemoveAsync(string userId, string symbol)
        {
            string sp = "sp_RemoveFromPortfolio";

            var param = new DynamicParameters();
            param.Add("@UserId", userId);
            param.Add("@Symbol", symbol);

            var dbResp = await _handler.ExecuteAsync<CommonResponseDTO>(sp, param);
            return dbResp;
        }
    }