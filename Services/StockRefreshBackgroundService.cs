using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinFlowAPI.Services
{
    public class StockRefreshBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockRefreshBackgroundService> _logger;
        private static bool _isRunning = false;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(3); // change to 2 if needed

        public StockRefreshBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<StockRefreshBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_isRunning)
            {
                _logger.LogWarning("Previous refresh still running. Skipping this cycle.");
            }
            else
            {
                try
                {
                    _isRunning = true;

                    using var scope = _scopeFactory.CreateScope();
                    var refreshService = scope.ServiceProvider
                        .GetRequiredService<StockCacheRefreshService>();

                    await refreshService.RefreshAsync();
                }
                finally
                {
                    _isRunning = false;
                }
            }
            _logger.LogInformation("Stock Refresh Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var refreshService = scope.ServiceProvider
                            .GetRequiredService<StockCacheRefreshService>();

                        int count = await refreshService.RefreshAsync();

                        _logger.LogInformation("Stock cache refreshed. {Count} records updated.", count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during stock cache refresh.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Stock Refresh Background Service stopped.");
        }
    }
}