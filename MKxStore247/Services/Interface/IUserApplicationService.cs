using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface IUserApplicationService
    {
        public Task<bool> IsFirstLogin(string userId);
        Task<UserApplication?> CompleteFirstLoginAsync(string userId, UserApplication userInfo);

        Task<(List<UserApplication> Users, int TotalCount)> GetUsersAsync(
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortBy = null,
            string? sortDirection = "ASC",
            string? statusFilter = null,
            string? roleFilter = null);

        Task<UserApplication?> GetUserByIdAsync(string id);

        Task<(bool Success, string[] Errors)> CreateUserAsync(UserApplication user, string password, List<string>? roles = null);

        Task<(bool Success, string[] Errors)> UpdateUserAsync(UserApplication user, List<string>? roles = null);

        Task<(bool Success, string[] Errors)> DeleteUserAsync(string id);

        Task<(bool Success, string Message)> ToggleUserStatusAsync(string id);

        Task<(bool Success, string[] Errors)> ResetPasswordAsync(string id, string newPassword);

        Task<(bool Success, string Message)> LockUserAsync(string id, DateTimeOffset? lockoutEnd = null);

        Task<(bool Success, string Message)> UnlockUserAsync(string id);

        Task<List<string>> GetUserRolesAsync(string userId);

        Task<List<Microsoft.AspNetCore.Identity.IdentityRole>> GetAllRolesAsync();

        Task<(bool Success, string[] Errors)> ConfirmEmailAsync(string id);

        Task<(bool Success, string[] Errors)> ConfirmPhoneAsync(string id);
    }
}
