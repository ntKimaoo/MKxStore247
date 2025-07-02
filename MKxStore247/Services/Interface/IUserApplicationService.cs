using Microsoft.AspNetCore.Identity;
using MKxStore247.Models;

namespace MKxStore247.Services.Interface
{
    public interface IUserApplicationService
    {
        public Task<bool> IsFirstLogin(string userId);
        Task<UserApplication?> CompleteFirstLoginAsync(string userId, UserApplication userInfo);
        Task<IEnumerable<UserApplication>> GetAllUsersAsync();
        Task<UserApplication?> GetUserByIdAsync(string id);
        Task<IdentityResult> CreateUserAsync(UserApplication user, string password);
        Task<IdentityResult> UpdateUserAsync(UserApplication user);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<IdentityResult> ToggleUserStatusAsync(string id);
        Task<IEnumerable<UserApplication>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> AddToRoleAsync(string userId, string role);
        Task<IdentityResult> RemoveFromRoleAsync(string userId, string role);
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}
