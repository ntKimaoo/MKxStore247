using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShopService _shopService;
        private readonly ICategoryProductService _categoryService;
        private readonly IFileUploadService _fileUploadService;
        private readonly UserManager<UserApplication> _userManager;
        public ProductController(
            IProductService productService,
            IShopService shopService,
            ICategoryProductService categoryService,
            IFileUploadService fileUploadService,
            UserManager<UserApplication> userManager)
        {
            _productService = productService;
            _shopService = shopService;
            _categoryService = categoryService;
            _fileUploadService = fileUploadService;
            _userManager = userManager;
        }
        [Route("/Product/{id}")]
        public async Task<IActionResult> ProductDetailsView(int id)
        {
            var product = await _productService.GetProductFullDetailsAsync(id);
            return View(product);
        }
        [Route("Search")]
        public async Task<IActionResult> Search(string searchTerm, int pageNumber = 1, int pageSize = 12, string sortBy = "ProductName", bool ascending = true)
        {
            try
            {
                var result = await _productService.GetPagedProductsAsync(
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    sortBy: sortBy,
                    ascending: ascending,
                    searchTerm: searchTerm
                );

                var model = (
                    Products: result.Products,
                    TotalCount: result.TotalCount,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    searchTerm: searchTerm ?? string.Empty
                );

                return View(model);
            }
            catch (Exception ex)
            {
                // Return empty result
                var emptyModel = (
                    Products: Enumerable.Empty<Product>(),
                    TotalCount: 0,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    searchTerm: searchTerm ?? string.Empty
                );

                return View(emptyModel);
            }
        }
        [Route("/ListProduct/{id}")]
        public async Task<IActionResult> ListProductFind(int id, string? searchProductName, int pageNumber = 1, int pageSize = 12, string sortBy = "ProductName", bool ascending = true)
        {
            try
            {
                var result = await _productService.GetPagedProductsByCategoryAsync(
                    categoryId: id,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    sortBy: sortBy,
                    ascending: ascending,
                    searchProductName: searchProductName
                );

                var model = (
                    Products: result.Products,
                    TotalCount: result.TotalCount,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    CategoryId: id,
                    SearchProductName: searchProductName ?? string.Empty
                );

                return View(model);
            }
            catch (Exception ex)
            {
                // Return empty result
                var emptyModel = (
                    Products: Enumerable.Empty<Product>(),
                    TotalCount: 0,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    CategoryId: id,
                    SearchProductName: searchProductName ?? string.Empty
                );

                return View(emptyModel);
            }
        }
        //[HttpPost("create")]
        //public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
        //{
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        var shop = await _shopService.GetShopsByOwnerAsync(user.Id);
        //        if (shop == null)
        //        {
        //            return Json(new { success = false, message = "Không tìm thấy thông tin shop" });
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            var errors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage)
        //                .ToList();
        //            return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
        //        }

        //        // Create product
        //        var product = new Product
        //        {
        //            ShopId = shop.ShopId,
        //            CategoryId = request.CategoryId,
        //            ProductName = request.ProductName,
        //            Description = request.Description,
        //            Price = request.Price,
        //            StockQuantity = request.StockQuantity,
        //            IsActive = request.IsActive,
        //            CreatedBy = User.Identity.Name,
        //            Images = new List<ProductImage>(),
        //            ProductOptions = new List<ProductOption>()
        //        };

        //        // Handle product images
        //        if (request.Images != null && request.ProductImages.Any())
        //        {
        //            var imageUrls = await _fileUploadService.UploadMultipleFilesAsync(
        //                request.ProductImages, "products");

        //            for (int i = 0; i < imageUrls.Count; i++)
        //            {
        //                product.Images.Add(new ProductImage
        //                {
        //                    ImageUrl = imageUrls[i],
        //                    IsMain = i == request.MainImageIndex,
        //                    IsActive = true
        //                });
        //            }
        //        }

        //        // Handle product options
        //        if (request.Options != null && request.Options.Any())
        //        {
        //            foreach (var optionRequest in request.Options)
        //            {
        //                var option = new ProductOption
        //                {
        //                    OptionName = optionRequest.Name,
        //                    Values = new List<ProductOptionValue>()
        //                };

        //                if (optionRequest.Values != null && optionRequest.Values.Any())
        //                {
        //                    foreach (var valueRequest in optionRequest.Values)
        //                    {
        //                        option.Values.Add(new ProductOptionValue
        //                        {
        //                            Value = valueRequest.Value,
        //                            StockQuantity = valueRequest.Stock
        //                        });
        //                    }
        //                }

        //                product.ProductOptions.Add(option);
        //            }
        //        }

        //        // Create product with all related data
        //        var createdProduct = await _productService.CreateAsync(product);

        //        return Json(new
        //        {
        //            success = true,
        //            message = "Sản phẩm đã được tạo thành công!",
        //            productId = createdProduct.ProductId,
        //            productName = createdProduct.ProductName
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
        //    }
        //}
    }

}
