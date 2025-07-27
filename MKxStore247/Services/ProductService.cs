using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Services
{
    public class ProductService : IProductService
    {
        private readonly MKxStore247Context _context;

        public ProductService(MKxStore247Context context)
        {
            _context = context;
        }

        #region CRUD Operations

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Set default values
                product.CreatedAt = DateTime.UtcNow;
                product.IsActive = true;
                product.IsDeleted = false;
                product.UpdatedAt = null;

                // Add product first
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Handle ProductImages if provided
                if (product.Images != null && product.Images.Any())
                {
                    // Ensure only one main image
                    var mainImageCount = product.Images.Count(img => img.IsMain);
                    if (mainImageCount == 0)
                    {
                        // Set first image as main if no main image specified
                        product.Images.First().IsMain = true;
                    }
                    else if (mainImageCount > 1)
                    {
                        // Set only first main image as true, others as false
                        bool isFirstMain = true;
                        foreach (var img in product.Images.Where(img => img.IsMain))
                        {
                            img.IsMain = isFirstMain;
                            isFirstMain = false;
                        }
                    }

                    foreach (var image in product.Images)
                    {
                        image.ProductId = product.ProductId;
                        image.IsActive = true;
                        _context.ProductImages.Add(image);
                    }
                }

                // Handle ProductOptions if provided
                if (product.ProductOptions != null && product.ProductOptions.Any())
                {
                    foreach (var option in product.ProductOptions)
                    {
                        option.ProductId = product.ProductId;
                        _context.ProductOptions.Add(option);

                        // Handle ProductOptionValues if provided
                        if (option.Values != null && option.Values.Any())
                        {
                            foreach (var optionValue in option.Values)
                            {
                                optionValue.OptionId = option.OptionId;
                                _context.ProductOptionValues.Add(optionValue);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return product with all related data
                return await GetProductFullDetailsAsync(product.ProductId);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null || existingProduct.IsDeleted)
                return null;

            // Update properties
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.IsActive = product.IsActive;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.UpdatedBy = product.UpdatedBy;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Filter and Search

        public async Task<IEnumerable<Product>> GetByShopIdAsync(int shopId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ShopId == shopId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => p.IsActive && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveProductsAsync();

            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p=>p.Images)
                .Where(p => (p.ProductName.Contains(searchTerm) ||
                           p.Description.Contains(searchTerm)) &&
                           p.IsActive && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice &&
                           p.IsActive && !p.IsDeleted)
                .ToListAsync();
        }

        #endregion

        #region Pagination

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(string searchTerm,
            int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true)
        {
            var query = _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted && p.ProductName.Contains(searchTerm));

            // Apply sorting
            query = ApplySorting(query, sortBy, ascending);

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsByShopAsync(
            int shopId, int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.ShopId == shopId && !p.IsDeleted);

            query = ApplySorting(query, sortBy, ascending);

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsByCategoryAsync(
            int categoryId, int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true, string? searchProductName = null)
        {
            var query = _context.Products
                .Include(p => p.Shop)
                .Include(p=>p.Images)
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted);
            // Add search functionality
            if (!string.IsNullOrEmpty(searchProductName))
            {
                query = query.Where(p => p.ProductName.Contains(searchProductName));
            }
            query = ApplySorting(query, sortBy, ascending);

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        #endregion

        #region Stock Management

        public async Task<Product> UpdateStockQuantityAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            product.StockQuantity = quantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> AddStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            product.StockQuantity += quantity;
            product.TotalImported += quantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> ReduceStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            if (product.StockQuantity < quantity)
                throw new InvalidOperationException("Insufficient stock quantity");

            product.StockQuantity -= quantity;
            product.SoldQuantity += quantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> IsInStockAsync(int productId, int requestedQuantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && !product.IsDeleted &&
                   product.IsActive && product.StockQuantity >= requestedQuantity;
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= threshold && p.IsActive && !p.IsDeleted)
                .ToListAsync();
        }

        #endregion

        #region Sales Management

        public async Task<Product> UpdateSoldQuantityAsync(int productId, int soldQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            product.SoldQuantity = soldQuantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetBestSellingProductsAsync(int topCount = 10)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.SoldQuantity)
                .Take(topCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBestSellingProductsByShopAsync(int shopId, int topCount = 10)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ShopId == shopId && p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.SoldQuantity)
                .Take(topCount)
                .ToListAsync();
        }

        #endregion

        #region Product Status

        public async Task<Product> ActivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            product.IsActive = true;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> DeactivateProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> ExistsAsync(int productId)
        {
            return await _context.Products
                .AnyAsync(p => p.ProductId == productId && !p.IsDeleted);
        }

        #endregion

        #region Related Data

        public async Task<Product> GetProductWithImagesAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        }

        public async Task<Product> GetProductWithOptionsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p => p.ProductOptions)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        }
        public async Task<Product> GetProductFullDetailsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductOptions)
                .ThenInclude(v=>v.Values)
                .Include(p => p.Ratings)
                .Include(p => p.StockImports)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted && p.IsActive);
        }

        #endregion

        #region Advanced Queries

        public async Task<IEnumerable<Product>> GetRecentProductsAsync(int count = 10)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p=>p.Images)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Include(p=>p.Images)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.SoldQuantity)
                .Take(8)
                .ToListAsync();
        }
        #endregion

        #region ImageProduct

        #region Product Images Management

        public async Task<ProductImage> AddProductImageAsync(int productId, ProductImage image)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            image.ProductId = productId;
            image.IsActive = true;

            // If this is set as main image, remove main flag from others
            if (image.IsMain)
            {
                var existingMainImages = await _context.ProductImages
                    .Where(img => img.ProductId == productId && img.IsMain)
                    .ToListAsync();

                foreach (var existingMain in existingMainImages)
                {
                    existingMain.IsMain = false;
                }
            }

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<bool> RemoveProductImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                return false;

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductImage> SetMainImageAsync(int productId, int imageId)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(img => img.ImageId == imageId && img.ProductId == productId);

            if (image == null)
                return null;

            // Remove main flag from other images
            var otherMainImages = await _context.ProductImages
                .Where(img => img.ProductId == productId && img.IsMain && img.ImageId != imageId)
                .ToListAsync();

            foreach (var otherImage in otherMainImages)
            {
                otherImage.IsMain = false;
            }

            // Set this image as main
            image.IsMain = true;
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId)
        {
            return await _context.ProductImages
                .Where(img => img.ProductId == productId && img.IsActive)
                .OrderByDescending(img => img.IsMain)
                .ToListAsync();
        }

        #endregion

        #region Option Product
        public async Task<ProductOption> AddProductOptionAsync(int productId, ProductOption option)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
                return null;

            option.ProductId = productId;
            _context.ProductOptions.Add(option);
            await _context.SaveChangesAsync();

            // Add option values if provided
            if (option.Values != null && option.Values.Any())
            {
                foreach (var optionValue in option.Values)
                {
                    optionValue.OptionId = option.OptionId;
                    _context.ProductOptionValues.Add(optionValue);
                }
                await _context.SaveChangesAsync();
            }

            return option;
        }

        public async Task<bool> RemoveProductOptionAsync(int optionId)
        {
            var option = await _context.ProductOptions
                .Include(o => o.Values)
                .FirstOrDefaultAsync(o => o.OptionId == optionId);

            if (option == null)
                return false;

            // Remove all option values first
            _context.ProductOptionValues.RemoveRange(option.Values);
            _context.ProductOptions.Remove(option);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductOptionValue> AddProductOptionValueAsync(int optionId, ProductOptionValue optionValue)
        {
            var option = await _context.ProductOptions.FindAsync(optionId);
            if (option == null)
                return null;

            optionValue.OptionId = optionId;
            _context.ProductOptionValues.Add(optionValue);
            await _context.SaveChangesAsync();
            return optionValue;
        }

        public async Task<bool> RemoveProductOptionValueAsync(int valueId)
        {
            var optionValue = await _context.ProductOptionValues.FindAsync(valueId);
            if (optionValue == null)
                return false;

            _context.ProductOptionValues.Remove(optionValue);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductOptionValue> UpdateOptionValueStockAsync(int valueId, int stockQuantity)
        {
            var optionValue = await _context.ProductOptionValues.FindAsync(valueId);
            if (optionValue == null)
                return null;

            optionValue.StockQuantity = stockQuantity;
            await _context.SaveChangesAsync();
            return optionValue;
        }

        #endregion

        #endregion

        #region Bulk Operations

        public async Task<bool> BulkUpdatePriceAsync(IEnumerable<int> productIds, decimal newPrice)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .ToListAsync();

            foreach (var product in products)
            {
                product.Price = newPrice;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BulkActivateAsync(IEnumerable<int> productIds)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .ToListAsync();

            foreach (var product in products)
            {
                product.IsActive = true;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BulkDeactivateAsync(IEnumerable<int> productIds)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .ToListAsync();

            foreach (var product in products)
            {
                product.IsActive = false;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BulkDeleteAsync(IEnumerable<int> productIds)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var product in products)
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Helper Methods

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy, bool ascending)
        {
            return sortBy.ToLower() switch
            {
                "productname" => ascending ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName),
                "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                "stockquantity" => ascending ? query.OrderBy(p => p.StockQuantity) : query.OrderByDescending(p => p.StockQuantity),
                "soldquantity" => ascending ? query.OrderBy(p => p.SoldQuantity) : query.OrderByDescending(p => p.SoldQuantity),
                _ => ascending ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName)
            };
        }

        #endregion
    }
}