using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auctions.Data;
using Auctions.Models;
using Auctions.Data.Services;
using System.Security.Claims;

namespace Auctions.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IListingsService _listingsService;
        private readonly IBidsService _bidsService;
        private readonly ICommentsService _commentsService;
        private readonly IImageStorageService _imageStorageService;
        private readonly ApplicationDbContext _context;

        public ListingsController(IListingsService listingsService, IBidsService bidsService, ICommentsService commentsService, IImageStorageService imageStorageService, ApplicationDbContext context)
        {
            _listingsService = listingsService;
            _bidsService = bidsService;
            _commentsService = commentsService;
            _imageStorageService = imageStorageService;
            _context = context;
        }

        // GET: Listings
        public async Task<IActionResult> Index(int? pageNumber, string searchString, int? categoryId)
        {
            await RefreshAuctionStatusesAsync();

            var applicationDbContext = _listingsService.GetAll();
            int pageSize = 3;

            await PopulateCategorySelectList(categoryId);
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategoryId"] = categoryId;

            if(!string.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(a => a.Title.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(l => l.CategoryId == categoryId.Value);
            }

            return View(await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.Status != AuctionStatus.Closed).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyListings(int? pageNumber)
        {
            await RefreshAuctionStatusesAsync();

            var applicationDbContext = _listingsService.GetAll();
            int pageSize = 3;

            await PopulateCategorySelectList();

            return View("Index", await PaginatedList<Listing>.CreateAsync(applicationDbContext.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> MyBids(int? pageNumber)
        {
            var applicationDbContext = _bidsService.GetAll();
            int pageSize = 3;

            return View(await PaginatedList<Bid>.CreateAsync(applicationDbContext.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _listingsService.GetById(id);

            if (listing == null)
            {
                return NotFound();
            }

            await RefreshAuctionStatusAsync(listing);

            return View(listing);
        }

        // GET: Listings/Create
        public async Task<IActionResult> Create()
        {
            await PopulateCategorySelectList();
            return View(new ListingVM
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(7)
            });
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingVM listing)
        {
            if (ModelState.IsValid && listing.Image != null)
            {
                if (listing.EndTime <= listing.StartTime)
                {
                    ModelState.AddModelError(nameof(listing.EndTime), "End time must be after start time.");
                    await PopulateCategorySelectList(listing.CategoryId);
                    return View(listing);
                }

                var imagePath = await _imageStorageService.UploadListingImageAsync(listing.Image);
                var startingPrice = listing.StartingPrice;
                var status = listing.StartTime <= DateTime.Now ? AuctionStatus.Active : AuctionStatus.Pending;

                var listObj = new Listing
                {
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = startingPrice,
                    StartingPrice = startingPrice,
                    CurrentPrice = startingPrice,
                    StartTime = listing.StartTime,
                    EndTime = listing.EndTime,
                    Status = status,
                    IdentityUserId = listing.IdentityUserId,
                    ImagePath = imagePath,
                    CategoryId = listing.CategoryId,
                };

                await _listingsService.Add(listObj);
                return RedirectToAction("Index");
            }

            await PopulateCategorySelectList(listing.CategoryId);
            return View(listing);
        }

        private async Task PopulateCategorySelectList(int? selectedCategoryId = null)
        {
            ViewData["Categories"] = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).AsNoTracking().ToListAsync(),
                "Id",
                "Name",
                selectedCategoryId);
        }

        [HttpPost]
        public async Task<ActionResult> AddBid([Bind("Id, Price, ListingId, IdentityUserId")] Bid bid)
        {
            var listing = await _listingsService.GetById(bid.ListingId);
            if (listing == null)
            {
                return NotFound();
            }

            await RefreshAuctionStatusAsync(listing);

            if (listing.Status == AuctionStatus.Active && ModelState.IsValid && bid.Price > listing.CurrentPrice)
            {
                await _bidsService.Add(bid);
                listing.Price = bid.Price;
                listing.CurrentPrice = bid.Price;
                await _listingsService.SaveChanges();
            }

            return View("Details", listing);
        }
        public async Task<ActionResult> CloseBidding(int id)
        {
            var listing = await _listingsService.GetById(id);
            if (listing == null)
            {
                return NotFound();
            }

            listing.IsSold = true;
            listing.Status = AuctionStatus.Closed;
            listing.WinnerUserId = listing.Bids?
                .OrderByDescending(b => b.Price)
                .FirstOrDefault()?.IdentityUserId;
            await _listingsService.SaveChanges();
            return View("Details", listing);
        }
        [HttpPost]
        public async Task<ActionResult> AddComment([Bind("Id, Content, ListingId, IdentityUserId")] Comment comment)
        {
            if(ModelState.IsValid)
            {
                await _commentsService.Add(comment);
            }
            var listing = await _listingsService.GetById(comment.ListingId);
            return View("Details", listing);
        }

        private async Task RefreshAuctionStatusesAsync()
        {
            var listings = await _context.Listings
                .Include(l => l.Bids)
                .Where(l => l.Status != AuctionStatus.Closed)
                .ToListAsync();

            var hasChanges = false;
            foreach (var listing in listings)
            {
                hasChanges |= UpdateAuctionStatus(listing);
            }

            if (hasChanges)
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task RefreshAuctionStatusAsync(Listing listing)
        {
            if (UpdateAuctionStatus(listing))
            {
                await _listingsService.SaveChanges();
            }
        }

        private static bool UpdateAuctionStatus(Listing listing)
        {
            var now = DateTime.Now;
            var originalStatus = listing.Status;
            var originalIsSold = listing.IsSold;
            var originalWinnerUserId = listing.WinnerUserId;

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

            return originalStatus != listing.Status
                || originalIsSold != listing.IsSold
                || originalWinnerUserId != listing.WinnerUserId;
        }

        //// GET: Listings/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var listing = await _context.Listings.FindAsync(id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", listing.IdentityUserId);
        //    return View(listing);
        //}

        //// POST: Listings/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,ImagePath,IsSold,IdentityUserId")] Listing listing)
        //{
        //    if (id != listing.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(listing);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ListingExists(listing.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", listing.IdentityUserId);
        //    return View(listing);
        //}

        //// GET: Listings/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var listing = await _context.Listings
        //        .Include(l => l.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (listing == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(listing);
        //}

        //// POST: Listings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var listing = await _context.Listings.FindAsync(id);
        //    if (listing != null)
        //    {
        //        _context.Listings.Remove(listing);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ListingExists(int id)
        //{
        //    return _context.Listings.Any(e => e.Id == id);
        //}
    }
}
