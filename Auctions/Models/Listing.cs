using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Auctions.Models
{
    public class Listing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double StartingPrice { get; set; }
        public double CurrentPrice { get; set; }
        public double MinimumBidIncrement { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AuctionStatus Status { get; set; }
        public string? WinnerUserId { get; set; }
        [Required]
        public string ContactPhoneNumber { get; set; } = string.Empty;
        public string ImagePath { get; set; }
        public bool IsSold { get; set; } = false;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }

        [ForeignKey("WinnerUserId")]
        public IdentityUser? Winner { get; set; }

        public List<Bid>? Bids { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<ListingImage>? ListingImages { get; set; }
    }
}
