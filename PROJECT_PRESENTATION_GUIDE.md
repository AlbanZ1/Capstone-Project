# AuctionsApp Project Presentation Guide

This document is a detailed presentation and defense guide for the **AuctionsApp** ASP.NET Core MVC capstone project. It explains the project from a technical and practical point of view, so you can confidently describe the architecture, features, workflows, implementation decisions, and challenges during a university presentation.

The project is an online auction marketplace where users can create auction listings, upload product images, place bids, see live bid updates, receive email notifications, and interact with listings through comments and localized user interface text.

---

# 1. Project Overview

## What AuctionsApp Is

**AuctionsApp** is a web-based auction marketplace built with **ASP.NET Core MVC**. It allows registered users to create auction listings and allows other users to bid on those listings until the auction ends. The application supports real-time bidding updates, multiple listing images, seller privacy, winner checkout information, localization, comments, countdown timers, and email notifications.

In simple terms, AuctionsApp works like a simplified online auction platform. A seller posts an item, chooses a starting price, sets auction start and end times, uploads images, and waits for bidders. Bidders can place bids while the auction is active. The highest valid bid becomes the current price, and when the auction closes, the highest bidder becomes the winner.

## Main Purpose of the System

The main purpose of AuctionsApp is to demonstrate how a complete real-world web application can be built using modern .NET technologies. It is not only a basic CRUD application. It includes authentication, database relationships, cloud storage, real-time communication, validation, email notifications, localization, and user-focused interface design.

The system is designed to solve several common marketplace requirements:

- Sellers need a way to publish auction items.
- Buyers need a way to browse active auctions.
- Bidders need immediate feedback when bids change.
- Users need secure authentication.
- Uploaded images need reliable external storage.
- Auction timing must be handled automatically.
- Sensitive seller contact information must stay private until appropriate.
- The interface should support multiple languages.

## What Problems It Solves

AuctionsApp solves the problem of manually managing auctions by automating listing creation, bidding, auction status changes, winner selection, and notifications.

Without this type of system, sellers would need to manually track bids, update prices, decide who won, contact bidders, and manage item images. AuctionsApp centralizes all those steps in one application.

The project also solves several technical problems:

- **Real-time updates:** SignalR updates connected users when a bid is placed.
- **Image management:** AWS S3 stores listing images outside the web server.
- **Auction rules:** Minimum bid increment prevents invalid low bids.
- **Fair bidding:** Anti-sniping extends auctions when bids arrive near the end.
- **Privacy:** Seller contact information is only visible to the seller and final winner.
- **Localization:** Users can switch between supported languages.

## Why an Auction Marketplace Was Chosen

An auction marketplace is a strong capstone project choice because it requires many realistic application features. It is more complex than a simple product catalog because it includes time-based behavior, user roles, validation rules, real-time updates, and transactional logic.

This type of project demonstrates:

- MVC design and separation of concerns.
- Entity Framework Core relationships and migrations.
- Authentication and authorization.
- File upload and cloud storage.
- Real-time web communication.
- User experience design.
- Business rules and validation.
- Production-oriented configuration using cloud services.

## Core Features Overview

The main features of AuctionsApp include:

- User registration and login.
- Google OAuth login.
- Auction listing creation.
- Multiple image upload per listing.
- AWS S3 image storage.
- Listing image carousel/gallery.
- Category support.
- Active, pending, and closed auction statuses.
- Bidding with minimum bid increment validation.
- SignalR live bidding updates.
- Countdown timers.
- Anti-sniping auction extension.
- Automatic auction closing.
- Winner determination.
- Comments on listings.
- Seller privacy.
- Winner checkout panel.
- SMTP email notifications.
- English, Albanian, and Macedonian localization.
- Responsive Bootstrap-based UI.
- FAQ/help system.

---

# 2. Technologies Used

## ASP.NET Core MVC

### Why It Was Used

ASP.NET Core MVC was used because it is a mature and powerful framework for building web applications with a clean separation between data, logic, and presentation. MVC stands for **Model-View-Controller**.

### What It Does in the Project

In AuctionsApp:

- **Models** represent entities such as `Listing`, `Bid`, `Comment`, `Category`, and `ListingImage`.
- **Views** are Razor pages that display HTML to users.
- **Controllers** handle incoming requests and coordinate actions between services, models, and views.

For example, when a user opens the listings page, the request goes to `ListingsController`, which gets listing data and returns the `Index.cshtml` view.

### Advantages

- Clear structure.
- Easy to maintain.
- Strong integration with Entity Framework Core and Identity.
- Supports Razor syntax for dynamic views.
- Good for server-rendered applications.
- Suitable for university and real-world projects.

## Entity Framework Core

### Why It Was Used

Entity Framework Core was used as the Object-Relational Mapper, or ORM. It allows the application to work with database records using C# classes instead of writing raw SQL for every operation.

### What It Does in the Project

EF Core maps models such as `Listing`, `Bid`, and `Comment` to database tables. It also handles relationships between models.

Example:

```csharp
public DbSet<Listing> Listings { get; set; }
public DbSet<Bid> Bids { get; set; }
public DbSet<Comment> Comments { get; set; }
public DbSet<Category> Categories { get; set; }
public DbSet<ListingImage> ListingImages { get; set; }
```

### Advantages

- Reduces manual SQL code.
- Supports LINQ queries.
- Handles database migrations.
- Manages relationships between tables.
- Works well with SQL Server and AWS RDS.

## SQL Server / AWS RDS

### Why It Was Used

SQL Server was used because it is a reliable relational database system that integrates well with ASP.NET Core and EF Core. AWS RDS provides managed database hosting in the cloud.

### What It Does in the Project

The database stores persistent application data:

- Users.
- Listings.
- Bids.
- Comments.
- Categories.
- Listing images.
- Identity authentication tables.

### Advantages

- Reliable data storage.
- Relational structure with foreign keys.
- Managed cloud hosting through AWS RDS.
- Backup and scaling options.
- Good compatibility with EF Core.

## SignalR

### Why It Was Used

SignalR was used because auctions require real-time communication. When a bid is placed, other users viewing the same listing should see the new price immediately without refreshing the page.

### What It Does in the Project

SignalR creates a live connection between the server and browser. In AuctionsApp, it is used for live bid updates on the listing details page.

When a bid is accepted, the server broadcasts an update to connected clients in the listing group.

### Advantages

- Real-time communication.
- Better user experience.
- Reduces manual page refreshes.
- Supports WebSockets when available.
- Integrates directly with ASP.NET Core.

## ASP.NET Identity

### Why It Was Used

ASP.NET Identity was used to manage users securely. Building authentication from scratch is risky, so using a standard framework improves security and reliability.

### What It Does in the Project

Identity handles:

- User registration.
- Login and logout.
- Password hashing.
- User accounts.
- Authentication cookies.
- Integration with Google login.

### Advantages

- Secure password handling.
- Built-in authentication system.
- Easy integration with MVC.
- Supports external providers such as Google.
- Provides Identity database tables automatically.

## Google OAuth Login

### Why It Was Used

Google OAuth login was added to improve usability. Many users prefer signing in with an existing Google account instead of creating a separate username and password.

### What It Does in the Project

The application redirects users to Google for authentication. After successful login, Google sends the user back to AuctionsApp, and ASP.NET Identity signs the user into the application.

### Advantages

- Faster login experience.
- Users do not need to remember another password.
- Google handles credential verification.
- Professional feature commonly used in real systems.

## AWS S3

### Why It Was Used

AWS S3 was used for image storage because uploaded images should not depend only on the web server file system. Cloud object storage is more reliable and scalable.

### What It Does in the Project

When a seller uploads listing images, the application uploads them to an S3 bucket and stores the public image URLs in the database.

### Advantages

- Scalable image storage.
- Separates media files from application code.
- Better for production deployment.
- Persistent storage even if the app server changes.
- Works well with multiple images per listing.

## Bootstrap

### Why It Was Used

Bootstrap was used to speed up responsive UI development and provide consistent styling patterns.

### What It Does in the Project

Bootstrap is used for:

- Layout grids.
- Buttons.
- Forms.
- Cards.
- Carousel/gallery.
- Responsive behavior.
- Modals and common UI components.

### Advantages

- Mobile-friendly by default.
- Consistent design system.
- Saves development time.
- Good browser support.

## JavaScript

### Why It Was Used

JavaScript was used for client-side interactivity that cannot be handled by static HTML alone.

### What It Does in the Project

JavaScript supports:

- Countdown timers.
- SignalR client connection.
- Live UI updates after bids.
- Payment option UI messages.
- Interactive frontend behavior.

### Advantages

- Updates page content without full reloads.
- Improves user experience.
- Works directly in the browser.
- Complements server-side MVC.

## Localization System

### Why It Was Used

Localization was used so the application can support multiple languages. This is important for usability in multilingual environments.

### What It Does in the Project

The app uses resource files and culture settings to display interface text in English, Albanian, and Macedonian.

### Advantages

- Makes the app accessible to more users.
- Demonstrates internationalization knowledge.
- Separates text from code.
- Allows easier language expansion later.

## SMTP Email Notifications

### Why It Was Used

SMTP email notifications were used to inform users about important auction events even when they are not actively viewing the website.

### What It Does in the Project

The email service can send notifications such as outbid alerts and winner-related messages.

### Advantages

- Improves user engagement.
- Provides important updates outside the browser.
- Useful for real auction workflows.
- Demonstrates integration with external communication services.

---

# 3. Project Architecture

## MVC Architecture

AuctionsApp follows the MVC architecture:

- **Model:** Represents data and business entities.
- **View:** Displays the user interface.
- **Controller:** Handles user requests and returns responses.

This separation makes the project easier to understand, test, and maintain.

## Controllers

Controllers are located in:

```text
Auctions/Controllers/
```

The most important controller is:

```text
ListingsController.cs
```

It handles:

- Displaying listings.
- Showing listing details.
- Creating listings.
- Adding bids.
- Closing auctions.
- Adding comments.
- Refreshing auction statuses.

Controllers do not directly render HTML. Instead, they choose which Razor view should be returned and pass the required model data to that view.

## Models

Models are located in:

```text
Auctions/Models/
```

Important models include:

- `Listing`
- `ListingImage`
- `Bid`
- `Comment`
- `Category`
- `ListingVM`
- `AuctionStatus`

Models describe the shape of the data and are used by EF Core to create database tables.

## Views

Views are located in:

```text
Auctions/Views/
```

Listing views are located in:

```text
Auctions/Views/Listings/
```

Important views:

- `Index.cshtml`: shows listing cards.
- `Details.cshtml`: shows one listing, bidding, comments, countdown, and gallery.
- `Create.cshtml`: form for creating a new listing.
- `MyBids.cshtml`: shows bids made by the current user.

Views use Razor syntax, which allows C# expressions inside HTML.

## Services

Services are located in:

```text
Auctions/Data/Services/
```

Services help keep controllers cleaner by separating data access and external integrations.

Important services include:

- `ListingsService`
- `BidsService`
- `CommentsService`
- `S3ImageStorageService`
- `EmailService`

For example, instead of writing all database queries directly inside the controller, `ListingsService` provides methods such as `GetAll`, `GetById`, `Add`, and `SaveChanges`.

## Hubs

SignalR hubs are located in:

```text
Auctions/Hubs/
```

The auction hub manages real-time communication between the server and connected browsers.

The main purpose of the hub is to allow users to join a listing-specific SignalR group and receive bid updates for that listing.

## Data Layer

The data layer is located in:

```text
Auctions/Data/
```

The central class is:

```text
ApplicationDbContext.cs
```

This class inherits from `IdentityDbContext`, meaning it contains both custom application tables and ASP.NET Identity tables.

## Dependency Injection

ASP.NET Core uses Dependency Injection to provide required services to controllers and other classes.

Example from a controller constructor:

```csharp
public ListingsController(
    IListingsService listingsService,
    IBidsService bidsService,
    ICommentsService commentsService,
    IImageStorageService imageStorageService,
    IEmailService emailService,
    ApplicationDbContext context,
    IHubContext<AuctionHub> auctionHubContext)
{
    _listingsService = listingsService;
    _bidsService = bidsService;
    _commentsService = commentsService;
    _imageStorageService = imageStorageService;
    _emailService = emailService;
    _context = context;
    _auctionHubContext = auctionHubContext;
}
```

This means the controller does not manually create dependencies. ASP.NET Core provides them automatically based on the configuration in `Program.cs`.

## Request Flow

A simple request flow looks like this:

```text
User -> Controller -> Service -> Database -> Service -> Controller -> View -> User
```

Example: viewing active listings:

1. User opens `/Listings`.
2. Request goes to `ListingsController.Index`.
3. Controller asks `ListingsService` for listings.
4. Service queries the database using EF Core.
5. Controller applies filters and pagination.
6. Controller returns `Index.cshtml`.
7. Browser displays listing cards.

Example: placing a bid:

1. User submits bid on details page.
2. Request goes to `ListingsController.AddBid`.
3. Controller loads the listing.
4. Auction status is refreshed.
5. Bid is validated.
6. Bid is saved to the database.
7. SignalR broadcasts the update.
8. Browser updates current price and bid count.

---

# 4. Database Structure

## Listing

### Purpose

The `Listing` model represents an auction item. It is the central entity of the application.

### Important Properties

- `Id`: Primary key.
- `Title`: Name of the auction item.
- `Description`: Detailed explanation of the item.
- `Price`: Current price, maintained for compatibility.
- `StartingPrice`: Initial auction price.
- `CurrentPrice`: Current highest bid amount.
- `MinimumBidIncrement`: Minimum amount required above current price.
- `StartTime`: When bidding starts.
- `EndTime`: When bidding ends.
- `Status`: Pending, Active, or Closed.
- `WinnerUserId`: User ID of the winner.
- `ContactPhoneNumber`: Seller phone number.
- `ImagePath`: Main cover image URL.
- `IsSold`: Indicates whether auction is closed/sold.
- `CategoryId`: Foreign key to category.
- `IdentityUserId`: Seller user ID.

### Relationships

A listing:

- Belongs to one seller.
- Belongs to one category.
- Can have many bids.
- Can have many comments.
- Can have many listing images.
- Can have one winner.

## ListingImage

### Purpose

The `ListingImage` model stores multiple images for a listing. This supports gallery and carousel functionality.

### Important Properties

- `Id`: Primary key.
- `ListingId`: Foreign key to the listing.
- `ImageUrl`: URL of the uploaded image in AWS S3.
- `IsPrimary`: Indicates whether this is the cover image.
- `DisplayOrder`: Controls image order in the gallery.

### Relationships

Many `ListingImage` records belong to one `Listing`.

The first uploaded image is saved in two places:

- `Listing.ImagePath`
- `ListingImage` with `IsPrimary = true`

This keeps backward compatibility with old listings and existing UI code.

## Bid

### Purpose

The `Bid` model represents a bid placed by a user on a listing.

### Important Properties

- `Id`: Primary key.
- `Price`: Bid amount.
- `ListingId`: Listing being bid on.
- `IdentityUserId`: User who placed the bid.
- `CreatedAt`: Time when the bid was placed.

### Relationships

A bid:

- Belongs to one listing.
- Belongs to one user.

The highest bid determines the winner when the auction closes.

## Comment

### Purpose

The `Comment` model allows users to ask questions or leave messages on listings.

### Important Properties

- `Id`: Primary key.
- `Content`: Comment text.
- `ListingId`: Related listing.
- `IdentityUserId`: User who posted the comment.

### Relationships

A comment:

- Belongs to one listing.
- Belongs to one user.

## Category

### Purpose

The `Category` model organizes listings into groups such as electronics, vehicles, furniture, clothing, books, and other.

### Important Properties

- `Id`: Primary key.
- `Name`: Category name.

### Relationships

One category can have many listings.

## IdentityUser

### Purpose

`IdentityUser` is provided by ASP.NET Identity. It represents registered application users.

### Important Properties

- `Id`: Unique user ID.
- `UserName`: Username.
- `Email`: Email address.
- `PasswordHash`: Securely hashed password.
- `PhoneNumber`: Optional phone number.

### Relationships

Users can:

- Create listings.
- Place bids.
- Add comments.
- Win auctions.

## EF Core Migrations

EF Core migrations are version-controlled database changes. Instead of manually changing the database, migrations describe schema changes in C#.

Example migration purpose:

```text
AddListingImages
```

This migration creates the `ListingImages` table and adds the foreign key relationship to `Listings`.

## Why Migrations Are Useful

Migrations are useful because they:

- Keep database structure synchronized with C# models.
- Allow changes to be reviewed before applying.
- Make database changes repeatable.
- Help developers and deployment environments stay consistent.
- Avoid manual database editing mistakes.

---

# 5. Authentication & Security

## Register/Login

Users can register and log in using ASP.NET Identity. Registration creates a user record in the Identity tables. Login verifies the user credentials and creates an authentication cookie.

Authenticated users can:

- Create listings.
- Place bids.
- Add comments.
- Close their own auctions.

Unauthenticated users can still browse listings, but they cannot perform protected actions.

## Google Login

Google login uses OAuth. The application redirects the user to Google, Google authenticates the user, and then redirects back to AuctionsApp.

The flow is:

1. User clicks Google login.
2. Browser goes to Google authentication page.
3. User approves login.
4. Google redirects back to the application.
5. ASP.NET Identity signs the user in.

This avoids storing a separate password for Google users.

## ASP.NET Identity

ASP.NET Identity provides:

- Secure password hashing.
- Authentication cookies.
- User tables.
- Login/logout functionality.
- External login support.

Using Identity is safer than creating custom authentication manually.

## Anti-Forgery Validation

Important POST actions use:

```csharp
[ValidateAntiForgeryToken]
```

This protects against Cross-Site Request Forgery attacks. Razor forms include an anti-forgery token, and the server verifies it when the form is submitted.

Examples:

- Creating a listing.
- Placing a bid.
- Adding a comment.

## Authorization

Protected actions use:

```csharp
[Authorize]
```

This means only logged-in users can access them.

Examples:

- Creating a listing requires login.
- Adding a bid requires login.
- Adding a comment requires login.

## Seller Privacy System

The seller contact information is not shown publicly to every visitor. This protects seller privacy.

The details page calculates whether the current user can view seller contact information.

Contact details are visible only when:

- The current user is the seller.
- The auction is closed and the current user is the winner.

## Winner-Only Contact Visibility

After an auction closes, only the winning bidder can see seller contact information. This supports the checkout process while keeping seller data private from non-winners.

This is important because marketplace users should not expose personal contact details to everyone.

## Validation Rules

The project includes several validation rules:

- Starting price must be greater than zero.
- Minimum bid increment must be greater than zero.
- End time must be after start time.
- Bid must be at least current price plus minimum increment.
- Sellers cannot bid on their own listings.
- Closed auctions do not accept bids.
- At least one image is required for new listings.
- Maximum 8 images per listing.
- Only image files are allowed.
- Each uploaded image must be within the configured size limit.

Validation improves data quality and prevents invalid system states.

---

# 6. Auction System Logic

## Creating Listings

The listing creation workflow starts when an authenticated seller opens the create listing page.

The seller enters:

- Title.
- Description.
- Starting price.
- Minimum bid increment.
- Category.
- Start time.
- End time.
- Contact phone number.
- One or more images.

When the form is submitted, the controller validates the input. If valid, images are uploaded to AWS S3, the listing is saved to the database, and the seller is redirected to the listings page.

## Bidding Logic

Bidding happens on the listing details page. A logged-in user enters a bid amount and submits the bid form.

The server checks:

1. The user is authenticated.
2. The listing exists.
3. The user is not the seller.
4. The auction is active.
5. The current time is between start and end time.
6. The bid amount is high enough.

If all checks pass:

- A new `Bid` record is created.
- The listing current price is updated.
- SignalR sends a live update to connected users.
- Email notification may be sent to the previous highest bidder.

## Minimum Bid Increment

The minimum bid increment prevents users from increasing the bid by extremely small amounts.

The minimum valid next bid is:

```text
CurrentPrice + MinimumBidIncrement
```

For example:

```text
Current price: $100
Minimum increment: $5
Minimum next bid: $105
```

If a user enters `$103`, the bid is rejected.

## Live Bidding

After a valid bid is saved, the application broadcasts a bid update using SignalR.

The browser updates:

- Current price.
- Minimum next bid.
- Bid count.
- Latest bid message.
- Countdown end time if anti-sniping extended the auction.

This creates a real-time auction experience.

## SignalR Updates

Each listing can have its own SignalR group. Users viewing listing details join that listing group.

When a bid is placed on listing ID 10, only users watching listing 10 need that update.

This is more efficient than broadcasting every bid to every connected user.

## Auction Countdown

Countdown timers show how much time remains before the auction closes.

The UI shows different states:

- Pending auctions show time until start.
- Active auctions show time until end.
- Closed auctions show closed status.

JavaScript updates the countdown in the browser.

## Anti-Sniping (+5 Minutes)

Anti-sniping prevents unfair last-second bidding. If a bid is placed near the end of the auction, the auction is extended by 5 minutes.

The logic is:

```text
If auction is active
and bid is placed before the end time
and remaining time is 5 minutes or less
then extend EndTime by 5 minutes
```

This gives other bidders a fair chance to respond.

## Auction Closing

Auctions close when:

- The end time passes.
- The seller manually closes bidding.

When the auction closes:

- Status becomes `Closed`.
- `IsSold` becomes true.
- Winner is selected from the highest bid.

## Winner Determination

The winner is the user who placed the highest bid.

The logic orders bids by price descending:

```csharp
listing.Bids?
    .OrderByDescending(b => b.Price)
    .FirstOrDefault()?.IdentityUserId;
```

The winner ID is stored in `WinnerUserId`.

## Full Flow From Listing Creation to Winner

1. Seller logs in.
2. Seller creates a listing.
3. Images are uploaded to AWS S3.
4. Listing is saved with status `Pending` or `Active`.
5. Buyers browse active listings.
6. Buyer opens details page.
7. Buyer submits a valid bid.
8. Bid is saved.
9. SignalR updates all connected viewers.
10. Previous highest bidder may receive an outbid email.
11. Countdown continues.
12. Last-minute bids may extend the auction.
13. Auction reaches end time.
14. System marks auction closed.
15. Highest bidder becomes winner.
16. Winner can see seller contact details and checkout panel.

---

# 7. Real-Time Features

## SignalR

SignalR is the main real-time technology in AuctionsApp. It allows the server to push updates to the browser immediately.

Traditional web applications require the browser to ask the server for updates. SignalR reverses this by allowing the server to notify the browser when something important happens.

## Real-Time Bid Updates

When a user places a bid:

1. Server validates the bid.
2. Bid is saved.
3. Listing price is updated.
4. SignalR sends an update to connected clients.
5. Browser updates the UI.

The user does not need to refresh the page.

## Countdown Synchronization

Countdown timers are calculated based on auction start and end times. If anti-sniping extends the auction, SignalR sends the updated end time to clients.

The browser then updates its countdown data.

## Live UI Updates

The live UI can update:

- Current price.
- Minimum next bid.
- Bid count.
- Latest bid text.
- Auction extension message.
- Countdown end time.

This makes the auction feel active and responsive.

## Why Real-Time Communication Is Important

Real-time communication is important in auction systems because users compete against each other. If one user places a bid, other users must know immediately. Without real-time updates, a bidder might think they are winning when they have already been outbid.

SignalR improves fairness, usability, and trust in the auction process.

---

# 8. Image Upload System

## Multiple Image Upload

Sellers can upload multiple images when creating a listing. The create form uses a file input with the `multiple` attribute:

```html
<input asp-for="Images" class="form-control" accept="image/*" multiple />
```

This allows the seller to select several images from their device.

## AWS S3 Integration

Uploaded images are sent to AWS S3 through the image storage service.

The service:

1. Validates that the file exists.
2. Reads AWS configuration.
3. Generates a unique object key.
4. Uploads the file to the S3 bucket.
5. Returns the public S3 URL.

The database stores only the image URL, not the image file itself.

## Carousel/Gallery

The details page displays listing images as a gallery.

If there is only one image:

- The image is shown normally.

If there are multiple images:

- A Bootstrap carousel is shown.
- Previous and next controls are available.
- Thumbnails are displayed below.

This gives bidders a better view of the item.

## Cover Image Logic

The first uploaded image becomes the cover image.

It is saved as:

```text
Listing.ImagePath
```

It is also saved as a `ListingImage` row with:

```text
IsPrimary = true
DisplayOrder = 0
```

Additional images have:

```text
IsPrimary = false
DisplayOrder = 1, 2, 3...
```

## Thumbnail System

Thumbnails are shown below the carousel. Each thumbnail is a small button linked to a specific carousel slide.

This improves navigation because the user can quickly jump between images instead of only using previous and next buttons.

## Validation Rules

Image upload validation includes:

- At least one image is required.
- Maximum 8 images per listing.
- Only image MIME types are accepted.
- Each file must be 5 MB or smaller.

These rules protect the application from invalid uploads and excessive storage usage.

---

# 9. Email Notification System

## SMTP Configuration

SMTP is used for sending emails from the application. SMTP settings should be provided through safe configuration methods such as environment variables or user secrets, not hardcoded secrets in `appsettings.json`.

Typical SMTP settings include:

- Host.
- Port.
- Username.
- Password.
- Sender email.
- SSL/TLS setting.

## Outbid Notifications

When a new valid bid is placed, the previous highest bidder may receive an email telling them that they have been outbid.

This email can include:

- Listing title.
- Previous highest bid.
- New highest bid.
- Link to the listing.

Outbid notifications encourage users to return and place another bid.

## Winner Notifications

Winner notifications are important because the winning bidder needs to know they won and may need to contact the seller.

The application supports winner checkout behavior in the UI, and email notifications can be used to make this process more complete.

## Why Notifications Are Important

Notifications are important because users may not keep the website open all the time. Email allows the system to communicate important events outside the active browser session.

In an auction marketplace, notifications improve:

- Engagement.
- Fairness.
- User awareness.
- Checkout completion.

---

# 10. Localization System

## English / Albanian / Macedonian Support

AuctionsApp supports multiple languages, including:

- English.
- Albanian.
- Macedonian.

This is handled through ASP.NET Core localization.

## Culture Cookies

The selected language can be stored in a culture cookie. When the user changes language, the application remembers the choice and uses that culture for future requests.

This means users do not need to select their language every time they visit the site.

## Resource/Localization Files

Localization text is stored in resource files under:

```text
Auctions/Resources/
```

Examples:

```text
SharedResource.sq.resx
SharedResource.mk.resx
```

The application uses `IStringLocalizer<SharedResource>` to access translated text in controllers and views.

Example:

```csharp
@inject IStringLocalizer<Auctions.SharedResource> L
```

Then text can be displayed like:

```csharp
@L["Create Listing"]
```

## Language Switcher

The UI includes a language switcher so users can change the current culture. This improves accessibility and makes the application more useful in multilingual contexts.

## Why Localization Improves Usability

Localization improves usability because users understand the system better in their preferred language.

It also demonstrates that the application was designed with broader user access in mind, not only a single-language audience.

---

# 11. Frontend & UI/UX

## Responsive Design

The application is designed to work on different screen sizes. A user can browse listings, view details, and place bids from desktop or mobile devices.

Responsive design is important because modern users access web applications from phones, tablets, laptops, and desktop computers.

## Bootstrap Layout

Bootstrap provides:

- Grid layout.
- Form styling.
- Buttons.
- Carousel.
- Responsive containers.
- Utility classes.

The project combines Bootstrap with custom CSS in:

```text
Auctions/wwwroot/css/site.css
```

## Animations/Microinteractions

The UI includes small interactive details such as:

- Card hover effects.
- Image zoom on hover.
- Status badges.
- Countdown updates.
- Live bid messages.

These microinteractions make the application feel more polished.

## Countdown UI

Countdown timers clearly show:

- Time until auction starts.
- Time until auction ends.
- Closed auction status.

This is important because auction timing is one of the most important parts of the user experience.

## FAQ Modal

The FAQ/help system explains how the auction process works. It helps new users understand listing creation, bidding, winning, and seller contact rules.

This reduces confusion and makes the app easier to use.

## Navbar

The navbar gives users quick access to important areas such as:

- Home.
- Listings.
- Create listing.
- Login/register.
- Language switching.

Navigation is important because users should always understand where they are and how to move through the app.

## Cards/Grid System

Listings are displayed as cards in a grid. Each card includes:

- Cover image.
- Category.
- Auction status.
- Title.
- Current price.
- Time remaining.
- Bid count.
- Details button.

Cards make it easy to scan multiple auctions quickly.

## Mobile Responsiveness

The layout adapts to smaller screens by stacking content vertically and resizing controls. This is especially important for the details page, where the image gallery, bidding panel, and comments must remain usable on mobile devices.

---

# 12. Important Features Added During Development

## Google Login

Google login was added to improve authentication usability and show integration with external identity providers.

## Multiple Image Gallery

The original listing image behavior was expanded to support multiple images using the new `ListingImage` model. The first image remains the cover image for backward compatibility.

## Anti-Sniping

Anti-sniping was added to prevent last-second unfair wins. If a bid arrives during the final 5 minutes, the auction extends by 5 minutes.

## Minimum Bid Increment

Minimum bid increment ensures that bids increase by meaningful amounts. This avoids very small increases like one cent unless the seller allows it.

## Email Notifications

Email notifications allow users to receive important auction updates even when they are not currently on the website.

## FAQ/Help System

The FAQ/help system explains how the platform works and improves user understanding.

## Seller Privacy

Seller contact information is hidden from the public and only visible to the seller or final winner.

## Winner Checkout UI

After an auction closes, the winning bidder sees checkout-related information, including seller contact details and payment option messages.

## Localization

Localization support was added for English, Albanian, and Macedonian, making the app more accessible.

## Live Countdown Timers

Countdown timers update in the browser and show users how much time remains for each auction.

---

# 13. Challenges Faced

## SignalR Synchronization

### Challenge

The challenge with SignalR was making sure only users viewing the correct listing receive bid updates for that listing.

### Solution

Listing-specific groups were used. When a user opens a listing details page, the browser joins that listing's SignalR group. When a bid is placed, the server sends the update only to that group.

## Pagination Issues

### Challenge

Pagination can become difficult when combined with filters such as category, search text, status, and user-specific listings.

### Solution

The controller keeps current filter values in `ViewData` and passes them back into pagination links. This keeps the user on the correct filtered view while moving between pages.

## Duplicate Comments on Refresh

### Challenge

If a comment form returns the same page after a POST, refreshing the browser can resubmit the comment.

### Solution

The application redirects after successful comment submission. This follows the Post/Redirect/Get pattern and prevents duplicate form submissions.

## Image Rendering

### Challenge

Images can have different sizes and aspect ratios. Without careful styling, listing cards and details pages can look inconsistent.

### Solution

CSS uses fixed aspect ratios, `object-fit`, and controlled maximum heights. Listing cards use cover image styling, while details pages use contain styling so the full image is visible.

## Authentication Issues

### Challenge

Some actions should only be available to logged-in users. Without proper authorization, unauthenticated users could access protected forms.

### Solution

`[Authorize]` attributes protect sensitive actions, and views also adjust UI based on whether the user is authenticated.

## Google OAuth Redirect Mismatch

### Challenge

Google OAuth requires the redirect URI in Google Cloud Console to exactly match the application callback URL. If it does not match, login fails.

### Solution

The correct callback URL must be configured in Google Cloud Console and application settings. Development and production URLs should both be registered if both are used.

## Database Migrations

### Challenge

Changing models requires the database schema to stay synchronized. Manual schema changes can cause errors.

### Solution

EF Core migrations are used. Each schema change is captured in a migration file and can be applied consistently using:

```bash
dotnet ef database update --project Auctions/Auctions.csproj
```

## Live Countdown Synchronization

### Challenge

Countdown timers need to remain accurate, especially when anti-sniping extends an auction.

### Solution

The server stores the authoritative `EndTime`. When an auction is extended, SignalR sends the updated end time to connected clients, and JavaScript updates the countdown.

---

# 14. Deployment Preparation

## AWS RDS

AWS RDS can host the SQL Server database in production.

Before deployment, the application needs:

- RDS SQL Server instance.
- Database connection string.
- Security group rules allowing application access.
- Applied EF Core migrations.

## AWS S3

AWS S3 stores listing images.

Before deployment, the application needs:

- S3 bucket.
- Correct bucket region.
- IAM permissions for uploading objects.
- Public access or signed URL strategy depending on security requirements.

## Environment Variables

Sensitive values should be stored in environment variables in production.

Examples:

- Database connection string.
- AWS access key.
- AWS secret key.
- S3 bucket name.
- SMTP password.
- Google OAuth client secret.

## User Secrets

For local development, .NET user secrets can store sensitive settings without committing them to Git.

Example:

```bash
dotnet user-secrets set "AWS:AccessKey" "your-access-key" --project Auctions/Auctions.csproj
```

Secrets should never be committed to `appsettings.json`.

## Production Configuration

Production configuration should include:

- Production database connection.
- S3 configuration.
- SMTP configuration.
- Google OAuth credentials.
- HTTPS configuration.
- Logging configuration.
- Secure cookie settings.

## What Is Needed Before Deploying Live

Before deploying live:

1. Configure production database.
2. Apply migrations.
3. Configure S3 bucket and permissions.
4. Configure SMTP provider.
5. Configure Google OAuth production redirect URI.
6. Set environment variables.
7. Enable HTTPS.
8. Test registration, login, image upload, bidding, SignalR, email, and localization.

---

# 15. Possible Future Improvements

## Real Payment Integration

The checkout panel could be connected to Stripe, PayPal, or another payment provider. This would allow winners to pay directly through the platform.

## Admin Dashboard

An admin dashboard could allow administrators to:

- Manage users.
- Review listings.
- Remove inappropriate content.
- Monitor auctions.
- View statistics.

## User Ratings

Buyers and sellers could rate each other after completed auctions. This would increase trust in the marketplace.

## Watchlist

Users could save listings to a watchlist and quickly return to auctions they are interested in.

## AI Recommendations

AI could recommend listings based on user activity, categories, bids, and search behavior.

## Push Notifications

Browser push notifications could alert users about outbids, ending auctions, and winning status.

## Advanced Analytics

Analytics could show:

- Most active categories.
- Average winning price.
- User bidding behavior.
- Seller performance.

## Chat System

A chat system could allow winners and sellers to communicate after an auction closes.

---

# 16. Presentation Questions & Answers

## 1. Why did you choose ASP.NET Core?

I chose ASP.NET Core because it is a modern, high-performance framework for building web applications. It supports MVC architecture, dependency injection, authentication, Entity Framework Core, SignalR, localization, and cloud deployment. It was suitable for this project because AuctionsApp needed many real-world web application features.

## 2. Why did you use MVC?

MVC separates the project into Models, Views, and Controllers. This makes the application easier to maintain because data, UI, and request-handling logic are not mixed together. For example, listing data is represented by models, listing pages are views, and listing operations are handled by `ListingsController`.

## 3. What is Entity Framework Core?

Entity Framework Core is an ORM that allows C# classes to be mapped to database tables. Instead of writing SQL manually for every query, I can use LINQ and C# objects. EF Core also supports migrations, which help keep the database schema synchronized with the application models.

## 4. Why use SQL Server or AWS RDS?

SQL Server is reliable and integrates well with ASP.NET Core and EF Core. AWS RDS provides managed cloud database hosting, which makes the application more production-ready because the database can run separately from the web server.

## 5. What is SignalR?

SignalR is an ASP.NET Core library for real-time communication. It allows the server to push updates to clients instantly. In AuctionsApp, SignalR updates the current price, bid count, and latest bid information when a new bid is placed.

## 6. Why is SignalR important for auctions?

Auctions depend on time-sensitive competition. If one user bids, other users must know immediately. SignalR prevents users from needing to refresh the page manually and makes the auction experience fairer and more interactive.

## 7. What is ASP.NET Identity?

ASP.NET Identity is the authentication and user management system used in the project. It handles registration, login, password hashing, authentication cookies, and external login support.

## 8. How does Google login work?

Google login uses OAuth. The user is redirected to Google, authenticates there, and Google redirects back to the application. ASP.NET Identity then signs the user into AuctionsApp.

## 9. Why did you use AWS S3?

AWS S3 was used to store uploaded listing images. This is better than storing images only on the web server because S3 is scalable, reliable, and suitable for production media storage.

## 10. How does multiple image upload work?

The create listing form allows multiple selected image files. The controller validates them, uploads each one to AWS S3, saves the first image as `Listing.ImagePath`, and saves all images in the `ListingImages` table.

## 11. Why keep `Listing.ImagePath`?

`Listing.ImagePath` was kept for backward compatibility. Older listings may only have one image URL stored there. Keeping it prevents old listings and existing UI code from breaking.

## 12. How is the cover image selected?

The first uploaded image becomes the cover image. It is stored in `Listing.ImagePath` and also in `ListingImages` with `IsPrimary = true`.

## 13. What is anti-sniping?

Anti-sniping is a rule that prevents users from winning by placing a bid at the last second. If a bid is placed during the final 5 minutes, the auction is extended by 5 minutes.

## 14. Why did you add minimum bid increment?

Minimum bid increment ensures that every new bid increases the price by a meaningful amount. It prevents users from raising the price by extremely small values.

## 15. How is the winner selected?

When the auction closes, the system looks at the bids for the listing and selects the highest bid. The user who placed that bid becomes the winner, and their user ID is stored in `WinnerUserId`.

## 16. Can a seller bid on their own listing?

No. The bidding logic checks if the current user is the seller. If they are, the bid is rejected.

## 17. What security measures exist?

The project uses ASP.NET Identity, authorization attributes, anti-forgery tokens, validation rules, password hashing, seller privacy checks, and safe configuration practices for secrets.

## 18. What is anti-forgery validation?

Anti-forgery validation protects forms from Cross-Site Request Forgery attacks. The server verifies that POST requests include a valid token generated by the application.

## 19. What is localization?

Localization means adapting the application to different languages and cultures. AuctionsApp supports English, Albanian, and Macedonian using resource files and culture settings.

## 20. Why is localization useful?

Localization makes the application easier to use for people who prefer different languages. It also makes the project more realistic because real applications often support multiple regions.

## 21. How are comments handled?

Authenticated users can post comments on listings. Comments are stored in the database and displayed on the details page with the comment author's username.

## 22. How does seller privacy work?

Seller contact information is hidden from regular users. It is visible only to the seller and to the winning bidder after the auction closes.

## 23. What is the winner checkout UI?

The winner checkout UI appears for the winning bidder after the auction closes. It shows seller contact information and payment option messages so the winner can proceed with the purchase.

## 24. How are emails sent?

The application uses an email service configured with SMTP. When an important auction event happens, such as a user being outbid, the email service sends a notification.

## 25. Why use services instead of putting everything in controllers?

Services keep the controller cleaner and separate responsibilities. For example, image upload logic belongs in an image storage service, not directly inside the controller.

## 26. What is dependency injection?

Dependency injection is a pattern where ASP.NET Core provides required services to classes automatically. This makes the code easier to test, maintain, and extend.

## 27. What is a migration?

A migration is a versioned database schema change. For example, adding the `ListingImages` table was done through a migration. Migrations help keep the database synchronized with the C# models.

## 28. How does the countdown work?

The countdown uses listing start and end times. JavaScript updates the displayed remaining time in the browser. If SignalR reports an updated end time due to anti-sniping, the countdown data is updated.

## 29. How does pagination work?

Listings are loaded in pages instead of displaying everything at once. This improves performance and keeps the UI clean. Filters are preserved while moving between pages.

## 30. Why use Bootstrap?

Bootstrap provides responsive UI components and layout utilities. It helped create forms, grids, cards, buttons, and the carousel efficiently.

## 31. What happens if an old listing has no `ListingImage` rows?

The application falls back to `Listing.ImagePath`. This means old listings still display correctly even without gallery records.

## 32. How are image files validated?

The controller checks that at least one image exists, that no more than 8 images are uploaded, that each file is an image type, and that each file is within the size limit.

## 33. Why not store images directly in the database?

Storing images in the database can make the database large and slower. It is better to store images in object storage like S3 and save only the image URLs in the database.

## 34. What happens when a bid is invalid?

The system adds a validation error and returns the details page with the error message. The bid is not saved.

## 35. What did you learn from this project?

I learned how to build a complete ASP.NET Core MVC application with authentication, database relationships, real-time updates, cloud image storage, localization, validation, and production-oriented configuration.

---

# 17. Project Workflow Summary

## User Registration

The user registers using the Identity registration page. ASP.NET Identity creates the user record and securely stores the password hash.

## Login

The user can log in with email/password or Google OAuth. After login, the application creates an authenticated session.

## Listing Creation

The seller opens the create listing page and enters item details, auction timing, price information, category, contact phone number, and images.

## Uploading Images

The selected images are validated and uploaded to AWS S3. The first image becomes the cover image, and all image URLs are saved in the database.

## Bidding

A buyer opens the details page and submits a bid. The server validates the bid against auction status, ownership, time, and minimum bid increment.

## Live Updates

After a valid bid, SignalR broadcasts the new price and bid information to all users viewing that listing.

## Auction Ending

When the auction end time is reached, the auction is marked as closed. The highest bidder becomes the winner.

## Winner Checkout

The winner can see seller contact information and checkout options. Other users cannot see private seller contact details.

## Email Notifications

Outbid users can receive emails informing them that they are no longer the highest bidder. Winner-related notifications can help complete the auction process.

---

# 18. Final Conclusion

AuctionsApp is a complete ASP.NET Core MVC capstone project that demonstrates many real-world software engineering concepts. It is more than a simple CRUD application because it includes authentication, real-time bidding, cloud image storage, database relationships, localization, privacy rules, email notifications, and a responsive user interface.

Through this project, important technical skills were gained:

- Building MVC applications with ASP.NET Core.
- Designing relational data models.
- Using Entity Framework Core and migrations.
- Implementing authentication with ASP.NET Identity.
- Integrating Google OAuth login.
- Uploading files to AWS S3.
- Using AWS RDS/SQL Server for persistent storage.
- Building real-time functionality with SignalR.
- Writing business rules for auctions.
- Handling validation and security.
- Designing responsive frontend pages.
- Supporting multiple languages with localization.
- Preparing an application for production deployment.

The real-world value of this project is that it models a practical online auction marketplace. It shows how sellers, buyers, bids, images, notifications, and auction timing can work together in a single system. It also demonstrates thoughtful software design decisions, such as keeping `Listing.ImagePath` for compatibility while adding a new `ListingImage` table for gallery support.

Overall, AuctionsApp proves the ability to build, explain, and defend a full-stack web application using modern Microsoft and cloud technologies.
