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
using Auctions.Hubs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Auctions.Controllers
{
    public class ListingsController : Controller
    {
        private const int ListingsPageSize = 9;
        private const int MaxListingImages = 8;
        private const long MaxListingImageBytes = 5 * 1024 * 1024;

        private readonly IListingsService _listingsService;
        private readonly IBidsService _bidsService;
        private readonly ICommentsService _commentsService;
        private readonly IImageStorageService _imageStorageService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<AuctionHub> _auctionHubContext;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ListingsController(IListingsService listingsService, IBidsService bidsService, ICommentsService commentsService, IImageStorageService imageStorageService, IEmailService emailService, ApplicationDbContext context, IHubContext<AuctionHub> auctionHubContext, IStringLocalizer<SharedResource> localizer)
        {
            _listingsService = listingsService;
            _bidsService = bidsService;
            _commentsService = commentsService;
            _imageStorageService = imageStorageService;
            _emailService = emailService;
            _context = context;
            _auctionHubContext = auctionHubContext;
            _localizer = localizer;
        }

        // GET: Listings
        public async Task<IActionResult> Index(int? pageNumber, string searchString, int? categoryId, AuctionStatus? statusFilter, string viewMode, bool mineOnly = false)
        {
            await RefreshAuctionStatusesAsync();

            var applicationDbContext = _listingsService.GetAll();

            await PopulateCategorySelectList(categoryId);
            PopulateListingViewState(nameof(Index), searchString, categoryId, statusFilter, viewMode, mineOnly);

            applicationDbContext = ApplyListingFilters(applicationDbContext, searchString, categoryId, statusFilter);

            if (mineOnly)
            {
                applicationDbContext = applicationDbContext.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            if (!statusFilter.HasValue)
            {
                applicationDbContext = applicationDbContext.Where(l => l.Status != AuctionStatus.Closed);
            }

            return View(await PaginatedList<Listing>.CreateAsync(
                applicationDbContext.OrderByDescending(l => l.Id).AsNoTracking(),
                pageNumber ?? 1,
                ListingsPageSize));
        }

        public async Task<IActionResult> MyListings(int? pageNumber, string searchString, int? categoryId, AuctionStatus? statusFilter, string viewMode)
        {
            await RefreshAuctionStatusesAsync();

            var applicationDbContext = _listingsService.GetAll();

            await PopulateCategorySelectList(categoryId);
            PopulateListingViewState(nameof(MyListings), searchString, categoryId, statusFilter, viewMode, true);

            applicationDbContext = ApplyListingFilters(applicationDbContext, searchString, categoryId, statusFilter);

            return View("Index", await PaginatedList<Listing>.CreateAsync(
                applicationDbContext
                    .Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .OrderByDescending(l => l.Id)
                    .AsNoTracking(),
                pageNumber ?? 1,
                ListingsPageSize));
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
        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingVM listing)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            listing.IdentityUserId = userId;
            ModelState.Remove(nameof(listing.IdentityUserId));

            var listingImages = GetSubmittedImages(listing);
            ValidateSubmittedImages(listingImages);

            if (ModelState.IsValid)
            {
                if (listing.StartingPrice <= 0)
                {
                    ModelState.AddModelError(nameof(listing.StartingPrice), _localizer["Starting price must be greater than 0."]);
                }

                if (listing.MinimumBidIncrement <= 0)
                {
                    ModelState.AddModelError(nameof(listing.MinimumBidIncrement), _localizer["Minimum bid increment must be greater than 0."]);
                }

                if (listing.EndTime <= listing.StartTime)
                {
                    ModelState.AddModelError(nameof(listing.EndTime), _localizer["End time must be after start time."]);
                }

                if (!ModelState.IsValid)
                {
                    await PopulateCategorySelectList(listing.CategoryId);
                    return View(listing);
                }

                var imageUrls = await _imageStorageService.UploadListingImagesAsync(listingImages);
                var startingPrice = listing.StartingPrice;
                var status = listing.StartTime <= DateTime.Now ? AuctionStatus.Active : AuctionStatus.Pending;

                var listObj = new Listing
                {
                    Title = listing.Title,
                    Description = listing.Description,
                    Price = startingPrice,
                    StartingPrice = startingPrice,
                    CurrentPrice = startingPrice,
                    MinimumBidIncrement = listing.MinimumBidIncrement,
                    StartTime = listing.StartTime,
                    EndTime = listing.EndTime,
                    Status = status,
                    IdentityUserId = userId,
                    ContactPhoneNumber = listing.ContactPhoneNumber,
                    ImagePath = imageUrls.First(),
                    CategoryId = listing.CategoryId,
                    ListingImages = imageUrls.Select((imageUrl, index) => new ListingImage
                    {
                        ImageUrl = imageUrl,
                        IsPrimary = index == 0,
                        DisplayOrder = index
                    }).ToList()
                };

                await _listingsService.Add(listObj);
                return RedirectToAction("Index");
            }

            await PopulateCategorySelectList(listing.CategoryId);
            return View(listing);
        }

        private static List<IFormFile> GetSubmittedImages(ListingVM listing)
        {
            var images = listing.Images?
                .Where(image => image != null && image.Length > 0)
                .ToList() ?? new List<IFormFile>();

            if (!images.Any() && listing.Image != null && listing.Image.Length > 0)
            {
                images.Add(listing.Image);
            }

            return images;
        }

        private void ValidateSubmittedImages(IReadOnlyCollection<IFormFile> images)
        {
            if (!images.Any())
            {
                ModelState.AddModelError(nameof(ListingVM.Images), _localizer["At least one listing image is required."]);
                return;
            }

            if (images.Count > MaxListingImages)
            {
                ModelState.AddModelError(nameof(ListingVM.Images), string.Format(_localizer["You can upload up to {0} images."], MaxListingImages));
            }

            foreach (var image in images)
            {
                if (string.IsNullOrWhiteSpace(image.ContentType) || !image.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(nameof(ListingVM.Images), _localizer["Only image files can be uploaded."]);
                    break;
                }

                if (image.Length > MaxListingImageBytes)
                {
                    ModelState.AddModelError(nameof(ListingVM.Images), _localizer["Each image must be 5 MB or smaller."]);
                    break;
                }
            }
        }

        private async Task PopulateCategorySelectList(int? selectedCategoryId = null)
        {
            ViewData["Categories"] = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).AsNoTracking().ToListAsync(),
                "Id",
                "Name",
                selectedCategoryId);
        }

        private static IQueryable<Listing> ApplyListingFilters(IQueryable<Listing> listings, string searchString, int? categoryId, AuctionStatus? statusFilter)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                listings = listings.Where(a => a.Title.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                listings = listings.Where(l => l.CategoryId == categoryId.Value);
            }

            if (statusFilter.HasValue)
            {
                listings = listings.Where(l => l.Status == statusFilter.Value);
            }

            return listings;
        }

        private void PopulateListingViewState(string action, string searchString, int? categoryId, AuctionStatus? statusFilter, string viewMode, bool mineOnly)
        {
            ViewData["CurrentListingAction"] = action;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategoryId"] = categoryId;
            ViewData["CurrentStatusFilter"] = statusFilter;
            ViewData["CurrentViewMode"] = viewMode;
            ViewData["MineOnly"] = mineOnly;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddBid(int listingId, double price)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var result = await _bidsService.PlaceBidAsync(listingId, price, userId, now);
            if (result.Listing == null)
            {
                return NotFound();
            }

            var listing = result.Listing;
            if (!result.Succeeded || result.Bid == null)
            {
                var errorMessage = result.ErrorMessage ?? "Unable to place bid.";
                ModelState.AddModelError(
                    nameof(price),
                    result.ErrorArguments.Length > 0
                        ? string.Format(_localizer[errorMessage], result.ErrorArguments)
                        : _localizer[errorMessage]);
                return View("Details", listing);
            }

            ViewData["AuctionExtended"] = result.AuctionExtended;

            var bidUpdate = new AuctionBidUpdate(
                listing.CurrentPrice,
                listing.CurrentPrice + listing.MinimumBidIncrement,
                result.Bid.Price,
                User.Identity?.Name ?? "Auction user",
                result.BidCount,
                result.Bid.CreatedAt,
                result.UpdatedEndTime,
                result.AuctionExtended);

            await _auctionHubContext.Clients
                .Group(AuctionHub.ListingGroupName(listing.Id))
                .SendAsync("BidPlaced", bidUpdate);

            if (result.PreviousHighestBidder != null && result.PreviousHighestBidder.Id != userId)
            {
                var listingUrl = Url.Action(nameof(Details), "Listings", new { id = listing.Id }, Request.Scheme)
                    ?? $"/Listings/Details/{listing.Id}";
                var bidderDisplayName = User.Identity?.Name ?? "Another bidder";
                var categoryText = listing.Category?.Name ?? "Uncategorized";
                var body = $"""
                    You have been outbid on {listing.Title}.

                    Listing: {listing.Title}
                    Category: {categoryText}
                    Previous highest bid: ${result.PreviousHighestBid:N2}
                    New highest bid: ${result.Bid.Price:N2}
                    New bidder: {bidderDisplayName}

                    View the listing:
                    {listingUrl}
                    """;

                await _emailService.SendEmailAsync(
                    result.PreviousHighestBidder.Email ?? string.Empty,
                    $"You have been outbid on {listing.Title}",
                    body);
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment([Bind("Id, Content, ListingId, IdentityUserId")] Comment comment)
        {
            if(ModelState.IsValid)
            {
                await _commentsService.Add(comment);
                return RedirectToAction(nameof(Details), new { id = comment.ListingId });
            }

            var listing = await _listingsService.GetById(comment.ListingId);
            if (listing == null)
            {
                return NotFound();
            }

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
