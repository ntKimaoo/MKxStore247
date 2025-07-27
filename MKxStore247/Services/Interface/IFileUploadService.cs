namespace MKxStore247.Services.Interface
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
        Task<List<string>> UploadMultipleFilesAsync(IEnumerable<IFormFile> files, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        Task<bool> DeleteMultipleFilesAsync(IEnumerable<string> filePaths);
        bool IsValidImageFile(IFormFile file);
        string GetFileExtension(IFormFile file);
        long GetFileSize(IFormFile file);
        Task<string> UploadBase64ImageAsync(string base64Image, string folder, string fileName = null);

    }
}
