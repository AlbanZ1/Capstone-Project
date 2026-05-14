using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Auctions.Data.Services
{
    public class S3ImageStorageService : IImageStorageService
    {
        private readonly IConfiguration _configuration;

        public S3ImageStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadListingImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Image file is required.", nameof(image));
            }

            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var bucketName = _configuration["AWS:S3:BucketName"];
            var region = _configuration["AWS:S3:Region"];

            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new InvalidOperationException("AWS access key is missing.");
            }

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("AWS secret key is missing.");
            }

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new InvalidOperationException("AWS S3 bucket name is missing.");
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new InvalidOperationException("AWS S3 region is missing.");
            }

            var objectKey = $"listing-images/{Guid.NewGuid():N}{Path.GetExtension(image.FileName)}";

            await using var stream = image.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                InputStream = stream,
                ContentType = image.ContentType
            };

            using var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
            await s3Client.PutObjectAsync(request);

            return $"https://{bucketName}.s3.{region}.amazonaws.com/{Uri.EscapeDataString(objectKey).Replace("%2F", "/")}";
        }
    }
}
