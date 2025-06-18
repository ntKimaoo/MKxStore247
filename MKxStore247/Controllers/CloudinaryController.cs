using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MKxStore247.Models.HelperModel;

namespace MKxStore247.Controllers
{
    public class CloudinaryController : Controller
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryController(IOptions<CloudinarySetting> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (file == null) return BadRequest("File trống.");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "test"
            };
            var upload = await _cloudinary.UploadAsync(uploadParams);

            ViewBag.PublicId = upload.PublicId;
            ViewBag.Url = upload.SecureUrl;

            return View("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string publicId, IFormFile newFile)
        {
            if (newFile == null) return BadRequest("File trống.");

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(newFile.FileName, newFile.OpenReadStream()),
                PublicId = publicId,
                Overwrite = true
            };
            var upload = await _cloudinary.UploadAsync(uploadParams);

            ViewBag.PublicId = upload.PublicId;
            ViewBag.Url = upload.SecureUrl;

            return View("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string publicId)
        {
            var deletion = await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            ViewBag.Result = deletion.Result == "ok" ? "Xóa ảnh thành công" : "Xóa ảnh thất bại";

            return View("Index");

        }
    }
}
