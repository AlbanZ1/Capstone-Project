using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class ListingImage
    {
        public int Id { get; set; }
        public int ListingId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }

        [ForeignKey("ListingId")]
        public Listing? Listing { get; set; }
    }
}
