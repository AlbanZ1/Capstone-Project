using Auctions.Data;
using Auctions.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Auctions.Models;
using Microsoft.Extensions.Configuration.AzureKeyVault;


var builder = WebApplication.CreateBuilder(args);

//// Add Azure Key Vault integration
//var keyVaultUrl = "https://auctionskeyvault.vault.azure.net/";
//builder.Configuration.AddAzureAppConfiguration(options =>
//{
//    options.Connect(new Uri(keyVaultUrl), new DefaultAzureCredential())
//           .ConfigureKeyVault(kv => { kv.SetCredential(new DefaultAzureCredential()); });
//});

//var sqlConnectionString = builder.Configuration["SQLConnectionString"];
//var storageConnectionString = builder.Configuration["AzureStorageConnectionString"];

// Read connection strings from appsettings.json
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"];

if (string.IsNullOrEmpty(sqlConnectionString))
{
    throw new InvalidOperationException("Connection string 'SQLConnectionString' not found in appsettings.json.");
}

if (string.IsNullOrEmpty(storageConnectionString))
{
    throw new InvalidOperationException("Connection string 'AzureStorage:ConnectionString' not found in appsettings.json.");
}

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IListingsService, ListingsService>();
builder.Services.AddScoped<IBidsService, BidsService>();
builder.Services.AddScoped<ICommentsService, CommentsService>();

builder.Services.Configure<StorageConfig>(options =>
{
    options.ConnectionString = storageConnectionString;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Listings}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
