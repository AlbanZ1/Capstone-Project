using Microsoft.AspNetCore.SignalR;

namespace Auctions.Hubs
{
    public class AuctionHub : Hub
    {
        public Task JoinListing(int listingId)
        {
            if (listingId <= 0)
            {
                throw new HubException("A valid listing is required.");
            }

            return Groups.AddToGroupAsync(Context.ConnectionId, ListingGroupName(listingId));
        }

        public static string ListingGroupName(int listingId)
        {
            return $"listing-{listingId}";
        }
    }

    public sealed record AuctionBidUpdate(
        double NewCurrentPrice,
        double BidAmount,
        string BidderDisplayName,
        int BidCount,
        DateTime BidTime);
}
