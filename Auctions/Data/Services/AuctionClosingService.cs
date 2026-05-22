using Auctions.Models;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Data.Services
{
    public class AuctionClosingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionClosingService> _logger;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(60);

        public AuctionClosingService(IServiceScopeFactory scopeFactory, ILogger<AuctionClosingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CloseExpiredAuctionsAsync(stoppingToken);
                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task CloseExpiredAuctionsAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var now = DateTime.Now;

                var expiredListings = await context.Listings
                    .Include(l => l.Bids)
                    .Where(l => l.Status == AuctionStatus.Active && l.EndTime <= now)
                    .ToListAsync(stoppingToken);

                foreach (var listing in expiredListings)
                {
                    var winningBid = listing.Bids?
                        .OrderByDescending(b => b.Price)
                        .ThenBy(b => b.CreatedAt)
                        .FirstOrDefault();

                    listing.Status = AuctionStatus.Closed;
                    listing.IsSold = true;
                    listing.WinnerUserId = winningBid?.IdentityUserId;
                }

                if (expiredListings.Count > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close expired auctions.");
            }
        }
    }
}
