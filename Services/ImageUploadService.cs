namespace ShopWebApp.Services
{
    public class ImageUploadService
    {
        private readonly IWebHostEnvironment _env;
        private const string UploadFolder = "images/products";

        public ImageUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> UploadAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowed.Contains(ext)) return null;

            var folder = Path.Combine(_env.WebRootPath, UploadFolder);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{UploadFolder}/{fileName}";
        }

        public void Delete(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }
}
