using Auctions.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Bid>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<ListingImage>()
                .HasOne(li => li.Listing)
                .WithMany(l => l.ListingImages)
                .HasForeignKey(li => li.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ListingImage>()
                .HasIndex(li => li.ListingId);

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Vehicles" },
                new Category { Id = 3, Name = "Furniture" },
                new Category { Id = 4, Name = "Clothing" },
                new Category { Id = 5, Name = "Books" },
                new Category { Id = 6, Name = "Other" }
            );
        }
    }
}
