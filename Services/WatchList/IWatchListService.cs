public interface IWatchListService
{
    Task<List<WatchlistItemDTO>> GetAsync(string userId);
    Task<CommonResponseDTO> AddAsync(AddWatchlistDTO dto, string userId);
    Task<CommonResponseDTO> RemoveAsync(string userId, string symbol);
}