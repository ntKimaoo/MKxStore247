using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MKxStore247.Data;
using MKxStore247.Models;

namespace MKxStore247.Services
{
    public class UserApplicationService : IUserApplicationService
    {
        private readonly MKxStore247Context _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserApplicationService> _logger;
        private readonly UserManager<UserApplication> _userManager;
        public UserApplicationService(
            MKxStore247Context context,
            IWebHostEnvironment environment,
            ILogger<UserApplicationService> logger,
            UserManager<UserApplication> userManager)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<bool> IsFirstLogin(String userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                return user?.IsFirstLogin ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking first login status for user {UserId}", userId);
                return false;
            }
        }


        public async Task<UserApplication?> CompleteFirstLoginAsync(string userId, UserApplication userInfo)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
                    return null;
                }

                // Cập nhật thông tin user
                user.FullName = userInfo.FullName;
                user.AvatarUrl = userInfo.AvatarUrl;
                user.Gender = userInfo.Gender;
                user.DateOfBirth = userInfo.DateOfBirth;
                user.PhoneNumber = userInfo.PhoneNumber;
                user.Address = userInfo.Address;
                user.IsFirstLogin = false;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("First login completed for user {UserId}", userId);
                    return user;
                }
                else
                {
                    _logger.LogError("Failed to update user {UserId}: {Errors}",
                        userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing first login for user {UserId}", userId);
                return null;
            }
        }
    }

}
