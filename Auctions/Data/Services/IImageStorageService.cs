using Microsoft.AspNetCore.Http;

namespace Auctions.Data.Services
{
    public interface IImageStorageService
    {
        Task<string> UploadListingImageAsync(IFormFile image);
    }
}
