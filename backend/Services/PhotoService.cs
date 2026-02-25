using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Models;
using Microsoft.Extensions.Options;

namespace ECommerce.Services
{
    public class PhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadProductImageAsync(IFormFile file, int productId)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid image file");

            if (!file.ContentType.StartsWith("image/"))
                throw new Exception("Only image files are allowed");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = $"products/{productId}"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            return result.SecureUrl.ToString();
        }

    }
}
