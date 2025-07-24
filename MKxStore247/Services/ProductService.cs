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
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = false;
            product.IsDeleted = false;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
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

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedProductsAsync(
            int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true)
        {
            var query = _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted);

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
            int categoryId, int pageNumber, int pageSize, string sortBy = "ProductName", bool ascending = true)
        {
            var query = _context.Products
                .Include(p => p.Shop)
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted);

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

        #endregion

        #region Advanced Queries

        public async Task<IEnumerable<Product>> GetRecentProductsAsync(int count = 10)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            // Assuming featured products are best selling or highly rated
            return await _context.Products
                .Include(p => p.Shop)
                .Include(p => p.Category)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.SoldQuantity)
                .Take(20)
                .ToListAsync();
        }
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