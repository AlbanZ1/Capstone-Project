using Auctions.Models;
using Microsoft.AspNetCore.Identity;

namespace Auctions.Data.Services
{
    public interface IBidsService
    {
        Task Add(Bid bid);
        Task<BidPlacementResult> PlaceBidAsync(int listingId, double price, string userId, DateTime now);
        IQueryable<Bid> GetAll();
    }

    public sealed record BidPlacementResult(
        bool Succeeded,
        string? ErrorMessage,
        Listing? Listing,
        Bid? Bid,
        IdentityUser? PreviousHighestBidder,
        double PreviousHighestBid,
        bool AuctionExtended,
        DateTime? UpdatedEndTime,
        int BidCount,
        object[] ErrorArguments)
    {
        public static BidPlacementResult Failed(string errorMessage, Listing? listing = null, params object[] errorArguments)
        {
            return new BidPlacementResult(false, errorMessage, listing, null, null, 0, false, null, 0, errorArguments);
        }
    }
}
