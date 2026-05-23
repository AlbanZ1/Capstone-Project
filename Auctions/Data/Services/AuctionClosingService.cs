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
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var now = DateTime.Now;

                var expiredListings = await context.Listings
                    .Include(l => l.User)
                    .Include(l => l.Bids!)
                    .ThenInclude(b => b.User)
                    .Where(l => l.Status == AuctionStatus.Active && l.EndTime <= now)
                    .ToListAsync(stoppingToken);

                var closedAuctions = new List<(Listing Listing, Bid? WinningBid)>();

                foreach (var listing in expiredListings)
                {
                    var winningBid = listing.Bids?
                        .OrderByDescending(b => b.Price)
                        .ThenBy(b => b.CreatedAt)
                        .FirstOrDefault();

                    listing.Status = AuctionStatus.Closed;
                    listing.IsSold = true;
                    listing.WinnerUserId = winningBid?.IdentityUserId;
                    closedAuctions.Add((listing, winningBid));
                }

                if (expiredListings.Count > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);

                    foreach (var (listing, winningBid) in closedAuctions)
                    {
                        await SendAuctionClosedEmailsAsync(emailService, listing, winningBid);
                    }
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

        private async Task SendAuctionClosedEmailsAsync(IEmailService emailService, Listing listing, Bid? winningBid)
        {
            var listingRoute = $"/Listings/Details/{listing.Id}";
            var sellerEmail = listing.User?.Email;

            if (winningBid?.User?.Email != null)
            {
                var winnerBody = $"""
                    You won the auction: {listing.Title}

                    Listing: {listing.Title}
                    Final winning price: ${winningBid.Price:N2}
                    Seller email: {sellerEmail ?? "Not available"}

                    Checkout/payment details will be available in the app.

                    View the listing:
                    {listingRoute}
                    """;

                await emailService.SendEmailAsync(
                    winningBid.User.Email,
                    $"You won the auction: {listing.Title}",
                    winnerBody);
            }

            if (sellerEmail != null)
            {
                var sellerResult = winningBid?.User?.Email != null
                    ? $"Winner email: {winningBid.User.Email}{Environment.NewLine}Final price: ${winningBid.Price:N2}"
                    : "This auction ended with no winner.";
                var sellerBody = $"""
                    Your auction ended: {listing.Title}

                    Listing: {listing.Title}
                    {sellerResult}

                    View the listing:
                    {listingRoute}
                    """;

                await emailService.SendEmailAsync(
                    sellerEmail,
                    $"Your auction ended: {listing.Title}",
                    sellerBody);
            }
        }
    }
}
