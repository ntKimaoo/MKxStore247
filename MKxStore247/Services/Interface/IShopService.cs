using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface IShopService
    {
        Task<ServiceResult<Shop>> CreateShopAsync(Shop shop);
        Task<ServiceResult<Shop>> UpdateShopAsync(Shop shop);
        Task<ServiceResult<bool>> DeleteShopAsync(int id);
        Task<Shop> GetShopByIdAsync(int id);
        Task<Shop> GetShopsByOwnerAsync(string ownerId);
        Task<List<Shop>> GetAllShopsAsync();
        Task<List<Shop>> GetActiveShopsAsync();
        Task<ServiceResult<bool>> ActivateShopAsync(int id);
        Task<ServiceResult<bool>> DeactivateShopAsync(int id);
        Task<bool> ShopExistsAsync(int id);
        Task<bool> IsShopOwnerAsync(int shopId, string userId);
        Task<bool> UpdateShopSettingsAsync(Shop shop);
        Task<IEnumerable<CategoryProduct>> GetCategoriesAsync();
        Task<bool> ShopExistsForOwnerAsync(string ownerId);
        Task<bool> IsShopUrlAvailableAsync(string shopUrl, int? excludeShopId = null);
        Task<ShopStatistics> GetShopStatisticsAsync(int shopId);
    }
    public class ShopStatistics
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalViews { get; set; }
    }
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult<T> SuccessResult(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResult<T> FailureResult(string message, List<string> errors = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}