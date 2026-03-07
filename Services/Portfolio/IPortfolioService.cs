public interface IPortfolioService
{
    Task<List<PortfolioItemDTO>> GetAsync(string userId);
    Task<CommonResponseDTO> AddAsync(AddPortfolioDTO dto, string userId);
    Task<CommonResponseDTO> RemoveAsync(string userId, string symbol);
}