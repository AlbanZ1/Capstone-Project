using Auctions.Models;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Data.Services
{
    public class BidsService : IBidsService
    {
        private readonly ApplicationDbContext _context;

        public BidsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<BidPlacementResult> PlaceBidAsync(int listingId, double price, string userId, DateTime now)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var listing = await LoadListingAsync(listingId);

                if (listing == null)
                {
                    return BidPlacementResult.Failed("Listing not found.");
                }

                RefreshAuctionStatus(listing, now);

                if (listing.IdentityUserId == userId)
                {
                    return BidPlacementResult.Failed("You cannot bid on your own listing.", listing);
                }

                if (listing.Status == AuctionStatus.Closed || listing.IsSold)
                {
                    return BidPlacementResult.Failed("This auction is closed and no longer accepts bids.", listing);
                }

                if (now < listing.StartTime)
                {
                    return BidPlacementResult.Failed("Bidding starts on {0}.", listing, listing.StartTime.ToString("g"));
                }

                if (now > listing.EndTime)
                {
                    return BidPlacementResult.Failed("This auction has ended and no longer accepts bids.", listing);
                }

                var minimumNextBid = listing.CurrentPrice + listing.MinimumBidIncrement;
                if (price < minimumNextBid)
                {
                    return BidPlacementResult.Failed("Your bid must be at least {0}.", listing, $"${minimumNextBid:N2}");
                }

                var previousHighestBid = listing.Bids?
                    .OrderByDescending(b => b.Price)
                    .ThenBy(b => b.CreatedAt)
                    .FirstOrDefault();
                var previousHighestBidder = previousHighestBid?.User;
                var oldHighestBid = listing.CurrentPrice;
                var bidCount = (listing.Bids?.Count ?? 0) + 1;

                var bid = new Bid
                {
                    Price = price,
                    ListingId = listing.Id,
                    IdentityUserId = userId,
                    CreatedAt = now
                };

                var auctionExtended = false;
                DateTime? updatedEndTime = null;
                if (listing.Status == AuctionStatus.Active
                    && !listing.IsSold
                    && now >= listing.StartTime
                    && now < listing.EndTime
                    && listing.EndTime - now <= TimeSpan.FromMinutes(5))
                {
                    listing.EndTime = now.AddMinutes(5);
                    auctionExtended = true;
                    updatedEndTime = listing.EndTime;
                }

                listing.Price = price;
                listing.CurrentPrice = price;

                _context.Bids.Add(bid);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new BidPlacementResult(
                    true,
                    null,
                    listing,
                    bid,
                    previousHighestBidder,
                    oldHighestBid,
                    auctionExtended,
                    updatedEndTime,
                    bidCount,
                    Array.Empty<object>());
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                var listing = await LoadListingAsync(listingId);
                return BidPlacementResult.Failed(
                    "Another bidder placed a bid before you. Please refresh and try again.",
                    listing);
            }
        }

        public IQueryable<Bid> GetAll()
        {
            var applicationDbContext = from a in _context.Bids.Include(b => b.Listing).ThenInclude(l => l!.User)
                                       select a;
            return applicationDbContext;
        }

        private async Task<Listing?> LoadListingAsync(int listingId)
        {
            return await _context.Listings
                .Include(l => l.User)
                .Include(l => l.Winner)
                .Include(l => l.Category)
                .Include(l => l.ListingImages)
                .Include(l => l.Comments!)
                .ThenInclude(c => c.User)
                .Include(l => l.Bids!)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(l => l.Id == listingId);
        }

        private static void RefreshAuctionStatus(Listing listing, DateTime now)
        {
            if (listing.EndTime <= now || listing.IsSold)
            {
                listing.Status = AuctionStatus.Closed;
                listing.IsSold = true;
                listing.WinnerUserId = listing.Bids?
                    .OrderByDescending(b => b.Price)
                    .FirstOrDefault()?.IdentityUserId;
            }
            else if (listing.StartTime <= now)
            {
                listing.Status = AuctionStatus.Active;
                listing.IsSold = false;
            }
            else
            {
                listing.Status = AuctionStatus.Pending;
                listing.IsSold = false;
            }
        }
    }
}
