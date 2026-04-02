namespace quanlybangiay.Helpers
{
    public static class FileUploadHelper
    {
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        /// <summary>
        /// Saves an uploaded file to wwwroot/uploads/{subfolder}/ and returns the public URL path.
        /// Returns null if the file is null, empty, or invalid.
        /// </summary>
        public static async Task<string?> SaveAsync(
            IFormFile? file,
            IWebHostEnvironment env,
            string subfolder)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException($"File vượt quá kích thước tối đa 5MB.");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                throw new InvalidOperationException($"Định dạng file không hợp lệ. Chỉ chấp nhận: jpg, jpeg, png, gif, webp.");

            var folder = Path.Combine(env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{ext.ToLower()}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{subfolder}/{fileName}";
        }

        /// <summary>
        /// Deletes a previously uploaded file from wwwroot given its public URL path.
        /// Only deletes files under /uploads/ to avoid accidental deletion.
        /// </summary>
        public static void Delete(IWebHostEnvironment env, string? urlPath)
        {
            if (string.IsNullOrWhiteSpace(urlPath) || !urlPath.StartsWith("/uploads/"))
                return;

            var fullPath = Path.Combine(env.WebRootPath, urlPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
