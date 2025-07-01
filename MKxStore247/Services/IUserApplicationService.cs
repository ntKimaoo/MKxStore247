using MKxStore247.Models;

namespace MKxStore247.Services
{
    public interface IUserApplicationService
    {
        public Task<bool> IsFirstLogin(String userId);
        Task<UserApplication?> CompleteFirstLoginAsync(string userId, UserApplication userInfo);
    }
}
