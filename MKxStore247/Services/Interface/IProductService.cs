using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface IProductService
    {
        // CRUD Operations
        Task<Product> GetByIdAsync(int productId);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int productId);
        Task<bool> SoftDeleteAsync(int productId);

        // Filter and Search
        Task<IEnumerable<Product>> GetByShopIdAsync(int shopId);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // Pagination
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(string searchTerm,
            int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true);

        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsByShopAsync(
            int shopId, int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true);

        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsByCategoryAsync(
            int categoryId, int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true, string? searchProductName = null);

        // Stock Management
        Task<Product> UpdateStockQuantityAsync(int productId, int quantity);
        Task<Product> AddStockAsync(int productId, int quantity);
        Task<Product> ReduceStockAsync(int productId, int quantity);
        Task<bool> IsInStockAsync(int productId, int requestedQuantity = 1);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);

        // Sales Management
        Task<Product> UpdateSoldQuantityAsync(int productId, int soldQuantity);
        Task<IEnumerable<Product>> GetBestSellingProductsAsync(int topCount = 10);
        Task<IEnumerable<Product>> GetBestSellingProductsByShopAsync(int shopId, int topCount = 10);

        // Product Status
        Task<Product> ActivateProductAsync(int productId);
        Task<Product> DeactivateProductAsync(int productId);
        Task<bool> ExistsAsync(int productId);

        // Related Data
        Task<Product> GetProductWithImagesAsync(int productId);
        Task<Product> GetProductWithOptionsAsync(int productId);
        Task<Product> GetProductFullDetailsAsync(int productId);

        // Advanced Queries
        Task<IEnumerable<Product>> GetRecentProductsAsync(int count = 10);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        // Product Images Management
        Task<ProductImage> AddProductImageAsync(int productId, ProductImage image);
        Task<bool> RemoveProductImageAsync(int imageId);
        Task<ProductImage> SetMainImageAsync(int productId, int imageId);
        Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId);

        // Product Options Management
        Task<ProductOption> AddProductOptionAsync(int productId, ProductOption option);
        Task<bool> RemoveProductOptionAsync(int optionId);
        Task<ProductOptionValue> AddProductOptionValueAsync(int optionId, ProductOptionValue optionValue);
        Task<bool> RemoveProductOptionValueAsync(int valueId);
        Task<ProductOptionValue> UpdateOptionValueStockAsync(int valueId, int stockQuantity);
        // Bulk Operations
        Task<bool> BulkUpdatePriceAsync(IEnumerable<int> productIds, decimal newPrice);
        Task<bool> BulkActivateAsync(IEnumerable<int> productIds);
        Task<bool> BulkDeactivateAsync(IEnumerable<int> productIds);
        Task<bool> BulkDeleteAsync(IEnumerable<int> productIds);
    }
}