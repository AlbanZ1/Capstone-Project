using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class ListingVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        [Required]
        [Display(Name = "Starting Price")]
        public double StartingPrice { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; } = DateTime.Now.AddDays(7);

        [Required]
        [Phone]
        [Display(Name = "Contact Phone Number")]
        public string ContactPhoneNumber { get; set; } = string.Empty;

        public IFormFile Image { get; set; }
        public bool IsSold { get; set; } = false;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        public string? IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public IdentityUser? User { get; set; }
    }
}
