using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;
using MKxStore247.Services.Interface;

namespace MKxStore247.Services.Implementation
{
    public class ShopService : IShopService
    {
        private readonly MKxStore247Context _context;
        private readonly ILogger<ShopService> _logger;

        public ShopService(MKxStore247Context context, ILogger<ShopService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<Shop>> CreateShopAsync(Shop shop)
        {
            try
            {
                // Validate shop data
                if (shop == null)
                {
                    return ServiceResult<Shop>.FailureResult("Shop data is required");
                }

                // Check if shop name already exists for this owner
                var existingShop = await _context.Shop
                    .FirstOrDefaultAsync(s => s.ShopName == shop.ShopName && s.OwnerId == shop.OwnerId &&shop.ShopUrl==s.ShopUrl);

                if (existingShop != null)
                {
                    return ServiceResult<Shop>.FailureResult("Shop Name or Url already exists");
                }

                // Add shop to database
                _context.Shop.Add(shop);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Shop created successfully: {shop.ShopName} by {shop.OwnerId}");
                return ServiceResult<Shop>.SuccessResult(shop, "Shop created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating shop: {shop?.ShopName}");
                return ServiceResult<Shop>.FailureResult("An error occurred while creating the shop");
            }
        }

        public async Task<ServiceResult<Shop>> UpdateShopAsync(Shop shop)
        {
            try
            {
                if (shop == null)
                {
                    return ServiceResult<Shop>.FailureResult("Shop data is required");
                }

                // Check if shop exists
                var existingShop = await _context.Shop.FindAsync(shop.ShopId);
                if (existingShop == null)
                {
                    return ServiceResult<Shop>.FailureResult("Shop not found");
                }

                // Check if shop name already exists for another shop by same owner
                var duplicateShop = await _context.Shop
                    .FirstOrDefaultAsync(s => s.ShopName == shop.ShopName && s.OwnerId == shop.OwnerId && s.ShopId != shop.ShopId);

                if (duplicateShop != null)
                {
                    return ServiceResult<Shop>.FailureResult("Shop name already exists");
                }

                // Update shop
                _context.Entry(existingShop).CurrentValues.SetValues(shop);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Shop updated successfully: {shop.ShopName}");
                return ServiceResult<Shop>.SuccessResult(shop, "Shop updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating shop: {shop?.ShopName}");
                return ServiceResult<Shop>.FailureResult("An error occurred while updating the shop");
            }
        }

        public async Task<ServiceResult<bool>> DeleteShopAsync(int id)
        {
            try
            {
                var shop = await _context.Shop.FindAsync(id);
                if (shop == null)
                {
                    return ServiceResult<bool>.FailureResult("Shop not found");
                }

                // Check if shop has products (optional validation)
                var hasProducts = await _context.Products.AnyAsync(p => p.ShopId == id);
                if (hasProducts)
                {
                    return ServiceResult<bool>.FailureResult("Cannot delete shop with existing products");
                }

                _context.Shop.Remove(shop);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Shop deleted successfully: {shop.ShopName}");
                return ServiceResult<bool>.SuccessResult(true, "Shop deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting shop with ID: {id}");
                return ServiceResult<bool>.FailureResult("An error occurred while deleting the shop");
            }
        }

        public async Task<Shop> GetShopByIdAsync(int id)
        {
            try
            {
                return await _context.Shop
                    .Include(s => s.MainCategoryProduct) // Include category if needed
                    .FirstOrDefaultAsync(s => s.ShopId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting shop with ID: {id}");
                return null;
            }
        }

        public async Task<Shop> GetShopsByOwnerAsync(string ownerId)
        {
            try
            {
                return await _context.Shop
                    .FirstOrDefaultAsync(s=>s.OwnerId.Equals(ownerId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting shops for owner: {ownerId}");
                return null;
            }
        }

        public async Task<List<Shop>> GetAllShopsAsync()
        {
            try
            {
                return await _context.Shop
                    .Include(s => s.MainCategoryProduct)
                    .Where(s => !s.IsDeleted)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all shops");
                return new List<Shop>();
            }
        }

        public async Task<List<Shop>> GetActiveShopsAsync()
        {
            try
            {
                return await _context.Shop
                    .Where(s => s.IsActive)
                    .Include(s => s.MainCategoryProduct)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active shops");
                return new List<Shop>();
            }
        }

        public async Task<ServiceResult<bool>> ActivateShopAsync(int id)
        {
            try
            {
                var shop = await _context.Shop.FindAsync(id);
                if (shop == null)
                {
                    return ServiceResult<bool>.FailureResult("Shop not found");
                }

                shop.IsActive = true;
                shop.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Shop activated: {shop.ShopName}");
                return ServiceResult<bool>.SuccessResult(true, "Shop activated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating shop with ID: {id}");
                return ServiceResult<bool>.FailureResult("An error occurred while activating the shop");
            }
        }

        public async Task<ServiceResult<bool>> DeactivateShopAsync(int id)
        {
            try
            {
                var shop = await _context.Shop.FindAsync(id);
                if (shop == null)
                {
                    return ServiceResult<bool>.FailureResult("Shop not found");
                }

                shop.IsActive = false;
                shop.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Shop deactivated: {shop.ShopName}");
                return ServiceResult<bool>.SuccessResult(true, "Shop deactivated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating shop with ID: {id}");
                return ServiceResult<bool>.FailureResult("An error occurred while deactivating the shop");
            }
        }

        public async Task<bool> ShopExistsAsync(int id)
        {
            try
            {
                return await _context.Shop.AnyAsync(s => s.ShopId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if shop exists with ID: {id}");
                return false;
            }
        }

        public async Task<bool> IsShopOwnerAsync(int shopId, string userId)
        {
            try
            {
                return await _context.Shop
                    .AnyAsync(s => s.ShopId == shopId && s.OwnerId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking shop ownership for shop ID: {shopId}, user: {userId}");
                return false;
            }
        }

        public Task<bool> UpdateShopSettingsAsync(Shop shop)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoryProduct>> GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ShopExistsForOwnerAsync(string ownerId)
        {
            try
            {
                return await _context.Shop.AnyAsync(s => s.OwnerId == ownerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if shop exists with ID: {ownerId}");
                return false;
            }
        }

        public Task<bool> IsShopUrlAvailableAsync(string shopUrl, int? excludeShopId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ShopStatistics> GetShopStatisticsAsync(int shopId)
        {
            try
            {
                var shop = await _context.Shop
                    .Include(s => s.Products)
                    .FirstOrDefaultAsync(s => s.ShopId == shopId);

                if (shop == null) return new ShopStatistics();

                return new ShopStatistics
                {
                    TotalProducts = shop.Products.Count,
                    ActiveProducts = shop.Products.Count(p => p.IsActive && !p.IsDeleted),
                    // TODO: Implement orders and revenue calculation when Order model is available
                    TotalOrders = 0,
                    PendingOrders = 0,
                    TotalRevenue = 0,
                    TotalViews = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shop statistics");
                throw;
            }
        }
    }
}