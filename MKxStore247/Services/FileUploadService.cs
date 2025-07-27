using MKxStore247.Services.Interface;

namespace MKxStore247.Services
{
    public class FileUploadService:IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;

        // Allowed image extensions
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

        // Max file size (5MB)
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        public FileUploadService(
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty or null");

                if (!IsValidImageFile(file))
                    throw new ArgumentException("Invalid image file format");

                if (file.Length > _maxFileSize)
                    throw new ArgumentException($"File size exceeds maximum limit of {_maxFileSize / (1024 * 1024)}MB");

                // Create upload directory if not exists
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{GetFileExtension(file)}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL
                return $"/uploads/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
                throw;
            }
        }

        public async Task<List<string>> UploadMultipleFilesAsync(IEnumerable<IFormFile> files, string folder)
        {
            var uploadedFiles = new List<string>();

            try
            {
                if (files == null || !files.Any())
                    return uploadedFiles;

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uploadedFile = await UploadFileAsync(file, folder);
                        uploadedFiles.Add(uploadedFile);
                    }
                }

                return uploadedFiles;
            }
            catch (Exception ex)
            {
                // Clean up any uploaded files if there's an error
                await DeleteMultipleFilesAsync(uploadedFiles);
                _logger.LogError(ex, "Error uploading multiple files");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                // Convert URL to physical path
                var physicalPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(physicalPath))
                {
                    await Task.Run(() => File.Delete(physicalPath));
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<bool> DeleteMultipleFilesAsync(IEnumerable<string> filePaths)
        {
            if (filePaths == null || !filePaths.Any())
                return true;

            var deletionTasks = filePaths.Select(DeleteFileAsync);
            var results = await Task.WhenAll(deletionTasks);

            return results.All(result => result);
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file extension
            var extension = GetFileExtension(file).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(extension))
                return false;

            // Check MIME type
            var allowedMimeTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png",
                "image/gif", "image/bmp", "image/webp"
            };

            return allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
        }

        public string GetFileExtension(IFormFile file)
        {
            return Path.GetExtension(file.FileName).ToLowerInvariant();
        }

        public long GetFileSize(IFormFile file)
        {
            return file?.Length ?? 0;
        }

        public async Task<string> UploadBase64ImageAsync(string base64Image, string folder, string fileName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                    throw new ArgumentException("Base64 image is empty");

                // Extract base64 data
                var base64Data = base64Image;
                var mimeType = "image/png"; // default

                if (base64Image.Contains(","))
                {
                    var parts = base64Image.Split(',');
                    var header = parts[0];
                    base64Data = parts[1];

                    // Extract MIME type
                    if (header.Contains("image/"))
                    {
                        var start = header.IndexOf("image/");
                        var end = header.IndexOf(";", start);
                        mimeType = header.Substring(start, end - start);
                    }
                }

                // Convert base64 to bytes
                var imageBytes = Convert.FromBase64String(base64Data);

                if (imageBytes.Length > _maxFileSize)
                    throw new ArgumentException($"Image size exceeds maximum limit of {_maxFileSize / (1024 * 1024)}MB");

                // Create upload directory
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate filename
                var extension = mimeType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    "image/bmp" => ".bmp",
                    "image/webp" => ".webp",
                    _ => ".png"
                };

                fileName = fileName ?? $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                await File.WriteAllBytesAsync(filePath, imageBytes);

                return $"/uploads/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading base64 image");
                throw;
            }
        }
    }
}
